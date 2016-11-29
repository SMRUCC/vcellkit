Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

<[Namespace]("data")>
Public Class DataVisualization : Inherits WebApp

    Public Sub New(main As PlatformEngine)
        MyBase.New(main)
    End Sub

    <ExportAPI("/data/taxonomy/sunburst.vb")>
    <[GET](GetType(String))>
    Public Function TaxonomySunburst(request As HttpRequest, response As HttpResponse) As Boolean

    End Function

    Public Overrides Function Page404() As String
        Return ""
    End Function
End Class
