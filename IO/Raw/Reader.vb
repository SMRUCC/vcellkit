Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Data.IO
Imports [Module] = Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute

Public Class Reader : Inherits Raw

    ReadOnly stream As BinaryDataReader
    ReadOnly moduleIndex As New Index(Of String)

    Dim offsetIndex As OrderSelector(Of NumericTagged(Of Dictionary(Of String, Long)))

    Sub New(input As Stream)
        stream = New BinaryDataReader(input)
    End Sub

    Public Function LoadIndex() As Reader
        Dim modules = GetModules _
            .ToDictionary(Function(prop)
                              Dim modAttr = prop.GetAttribute(Of [Module])

                              If modAttr Is Nothing OrElse modAttr.Name.StringEmpty Then
                                  Return prop.Name
                              Else
                                  Return modAttr.Name
                              End If
                          End Function)

        Call stream.Seek(0, SeekOrigin.Begin)
        Call moduleIndex.Clear()

        If stream.ReadString(Magic.Length) <> Magic Then
            Throw New InvalidDataException("Invalid magic string!")
        Else
            For i As Integer = 0 To modules.Count - 1
                Dim name = stream.ReadDwordLenString
                Dim n = stream.ReadInt32
                Dim list As New Index(Of String)

                For j As Integer = 0 To n - 1
                    Call list.Add(stream.ReadString(BinaryStringFormat.ZeroTerminated))
                Next

                Call moduleIndex.Add(name)
                Call modules(name).SetValue(Me, list)
            Next
        End If

        Return Me
    End Function

    Public Function Read(time#, module$) As Double()

    End Function
End Class