Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' Module loader
''' </summary>
Public Module Loader

    <Extension>
    Public Function CreateEnvironment(cell As CellularModule) As Vessel
        Dim channels As New List(Of Channel)
        Dim massTable As New Dictionary(Of String, Factor)

        ' 先构建转录过程
        For Each cd As CentralDogma In cell.Genotype.centralDogmas

        Next
    End Function

    ''' <summary>
    ''' 重置反应环境模拟器之中的内容
    ''' </summary>
    ''' <param name="envir"></param>
    ''' <param name="massInit"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Reset(envir As Vessel, massInit As Dictionary(Of String, Double)) As Vessel
        For Each mass As Factor In envir.Mass
            mass.Value = massInit(mass.ID)
        Next

        Return envir
    End Function
End Module
