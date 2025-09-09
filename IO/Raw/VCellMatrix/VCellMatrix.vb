Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Public Class VCellMatrix

    ''' <summary>
    ''' the molecule expression data
    ''' </summary>
    ''' <returns></returns>
    Public Property expressionData As DataFrameRow()
    ''' <summary>
    ''' the biological process activity expression data
    ''' </summary>
    ''' <returns></returns>
    Public Property fluxomics As DataFrameRow()

    Public Property cellularGraph As FluxEdge()
    Public Property modules As Dictionary(Of String, String())
    Public Property compartmentIds As String()

End Class
