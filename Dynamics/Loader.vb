Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' Module loader
''' </summary>
Public Class Loader

    Dim define As Definition
    Dim massTable As New MassTable

    Sub New(define As Definition)
        Me.define = define
    End Sub

    Public Function CreateEnvironment(cell As CellularModule) As Vessel
        Dim channels As New List(Of Channel)
        Dim rnaMatrix = cell.Genotype.RNAMatrix.ToDictionary(Function(r) r.geneID)
        Dim proteinMatrix = cell.Genotype.ProteinMatrix.ToDictionary(Function(r) r.proteinID)

        ' 先构建一般性的中心法则过程
        For Each cd As CentralDogma In cell.Genotype.centralDogmas
            Call massTable.AddNew(cd.geneID)
            Call massTable.AddNew(cd.RNA.Name)
            Call massTable.AddNew(cd.polypeptide)

            channels += New Channel(transcriptionTemplate(cd.geneID, rnaMatrix), {massTable.variable(cd.RNA.Name)})
            channels += New Channel(translationTemplate(cd.RNA.Name, proteinMatrix), {massTable.variable(cd.polypeptide)})
        Next

        ' 构建酶成熟的过程
        For Each complex As Protein In cell.Phenotype.proteins
            For Each compound In complex.compounds
                If Not massTable.Exists(compound) Then
                    Call massTable.AddNew(compound)
                End If
            Next
            For Each peptide In complex.polypeptides
                If Not massTable.Exists(peptide) Then
                    Throw New MissingMemberException(peptide)
                End If
            Next

            channels += New Channel(massTable.variables(complex), {massTable.variable(complex.ProteinID)})
        Next

        ' 构建代谢网络
        For Each reaction As Reaction In cell.Phenotype.fluxes
            For Each compound In reaction.AllCompounds
                If Not massTable.Exists(compound) Then
                    Call massTable.AddNew(compound)
                End If
            Next

            Dim left = massTable.variables(reaction.substrates)
            Dim right = massTable.variables(reaction.products)

            channels += New Channel(left, right) With {
                .bounds = reaction.bounds,
                .ID = reaction.ID,
                .Forward = New Regulation With {
                    .Activation = massTable.variables(reaction.enzyme, 1)
                },
                .Reverse = New Regulation With {.baseline = 10}
            }
        Next

        Return New Vessel With {
            .Channels = channels,
            .Mass = massTable.GetAll.Values.ToArray
        }
    End Function

    Private Function transcriptionTemplate(geneID$, matrix As Dictionary(Of String, RNAComposition)) As Variable()
        Return matrix(geneID) _
            .Where(Function(i) i.Value > 0) _
            .Select(Function(base) massTable.variable(base.Name, base.Value)) _
            .AsList + massTable.template(geneID)
    End Function

    Private Function translationTemplate(mRNA$, matrix As Dictionary(Of String, ProteinComposition)) As Variable()
        Return matrix(mRNA) _
            .Where(Function(i) i.Value > 0) _
            .Select(Function(aa) massTable.variable(aa.Name, aa.Value)) _
            .AsList + massTable.template(mRNA)
    End Function
End Class

Public Class MassTable : Implements IRepository(Of String, Factor)

    Dim massTable As New Dictionary(Of String, Factor)

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