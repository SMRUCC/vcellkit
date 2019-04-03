Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics

''' <summary>
''' Module loader
''' </summary>
Public Class Loader

    Dim define As Definition

    Sub New(define As Definition)
        Me.define = define
    End Sub

    Public Function CreateEnvironment(cell As CellularModule) As Vessel
        Dim channels As New List(Of Channel)
        Dim massTable As New Dictionary(Of String, Factor)
        Dim rnaMatrix = cell.Genotype.RNAMatrix.ToDictionary(Function(r) r.geneID)
        Dim proteinMatrix = cell.Genotype.ProteinMatrix.ToDictionary(Function(r) r.proteinID)

        ' 先构建一般性的中心法则过程
        For Each cd As CentralDogma In cell.Genotype.centralDogmas
            Call massTable.Add(cd.geneID, cd.geneID)
            Call massTable.Add(cd.RNA.Name, cd.RNA.Name)
            Call massTable.Add(cd.polypeptide, cd.polypeptide)

            channels += New Channel(massTable.transcriptionTemplate(cd.geneID, rnaMatrix), {massTable.variable(cd.RNA.Name)})
            channels += New Channel(massTable.translationTemplate(cd.RNA.Name, proteinMatrix), {massTable.variable(cd.polypeptide)})
        Next

        ' 构建酶成熟的过程
        For Each complex As Protein In cell.Phenotype.proteins
            For Each compound In complex.compounds
                If Not massTable.ContainsKey(compound) Then
                    massTable(compound) = compound
                End If
            Next
            For Each peptide In complex.polypeptides
                If Not massTable.ContainsKey(peptide) Then
                    Throw New MissingMemberException(peptide)
                End If
            Next

            channels += New Channel(massTable.variables(complex), {massTable.variable(complex.ProteinID)})
        Next

        ' 构建代谢网络
        For Each reaction As Reaction In cell.Phenotype.fluxes
            For Each compound In reaction.AllCompounds
                If Not massTable.ContainsKey(compound) Then
                    massTable(compound) = compound
                End If
            Next

            Dim left = reaction.substrates.variables(massTable)
            Dim right = reaction.products.variables(massTable)

            channels += New Channel(left, right) With {
                .bounds = reaction.bounds,
                .ID = reaction.ID,
                .Forward = New Regulation With {
                    .Activation = reaction.enzyme.variables(massTable, 1)
                },
                .Reverse = New Regulation With {.baseline = 10}
            }
        Next

        Return New Vessel With {
            .Channels = channels,
            .Mass = massTable.Values.ToArray
        }
    End Function

    <Extension>
    Private Function transcriptionTemplate(massTable As Dictionary(Of String, Factor), geneID$, matrix As Dictionary(Of String, RNAComposition), define As Definition) As Variable()
        Return matrix(geneID) _
            .Where(Function(i) i.Value > 0) _
            .Select(Function(base) massTable.variable(base.Name, base.Value)) _
            .AsList + massTable.template(geneID)
    End Function

    <Extension>
    Private Function translationTemplate(massTable As Dictionary(Of String, Factor), mRNA$, matrix As Dictionary(Of String, ProteinComposition)) As Variable()
        Return matrix(mRNA) _
            .Where(Function(i) i.Value > 0) _
            .Select(Function(aa) massTable.variable(aa.Name, aa.Value)) _
            .AsList + massTable.template(mRNA)
    End Function
End Class

Public Class MassTable : Implements IRepository(Of String, Factor)

    Dim massTable As Dictionary(Of String, Factor)

    Public Sub Delete(key As String) Implements IRepositoryWrite(Of String, Factor).Delete
        If massTable.ContainsKey(key) Then
            Call massTable.Remove(key)
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub AddOrUpdate(entity As Factor, key As String) Implements IRepositoryWrite(Of String, Factor).AddOrUpdate
        massTable(key) = entity
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function variables(compounds As IEnumerable(Of String), factor As Double) As IEnumerable(Of Variable)
        Return compounds.Select(Function(cpd) Me.variable(cpd, factor))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function variables(compounds As IEnumerable(Of FactorString(Of Double))) As IEnumerable(Of Variable)
        Return compounds.Select(Function(cpd) Me.variable(cpd.text, cpd.factor))
    End Function

    Public Iterator Function variables(complex As Protein) As IEnumerable(Of Variable)
        For Each compound In complex.compounds
            Yield Me.variable(compound)
        Next
        For Each peptide In complex.polypeptides
            Yield Me.variable(peptide)
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function variable(mass As String, Optional coefficient As Double = 1) As Variable
        Return New Variable(massTable(mass), coefficient, False)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function template(mass As String) As Variable
        Return New Variable(massTable(mass), 1, True)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, Factor).Exists
        Return massTable.ContainsKey(key)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByKey(key As String) As Factor Implements IRepositoryRead(Of String, Factor).GetByKey
        Return massTable.TryGetValue(key)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetWhere(clause As Func(Of Factor, Boolean)) As IReadOnlyDictionary(Of String, Factor) Implements IRepositoryRead(Of String, Factor).GetWhere
        Return massTable.Values.Where(clause).ToDictionary
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAll() As IReadOnlyDictionary(Of String, Factor) Implements IRepositoryRead(Of String, Factor).GetAll
        Return massTable
    End Function

    Public Function AddNew(entity As Factor) As String Implements IRepositoryWrite(Of String, Factor).AddNew
        massTable(entity.ID) = entity
        Return entity.ID
    End Function
End Class