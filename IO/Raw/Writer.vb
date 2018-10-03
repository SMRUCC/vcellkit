Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports [Module] = Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute

''' <summary>
''' 写数据模块
''' </summary>
Public Class Writer : Inherits Raw

    ReadOnly stream As BinaryDataWriter
    ReadOnly modules As New Dictionary(Of String, Index(Of String))
    ReadOnly moduleIndex As New Index(Of String)

    Sub New(output As Stream)
        stream = New BinaryDataWriter(output, Encodings.UTF8WithoutBOM)
    End Sub

    ''' <summary>
    ''' 将编号列表写入的原始文件之中
    ''' </summary>
    ''' <returns></returns>
    Public Function Init() As Writer
        Dim modules As PropertyInfo() = Me.GetModules

        Call stream.Seek(0, SeekOrigin.Begin)
        Call stream.Write(Raw.Magic, BinaryStringFormat.NoPrefixOrTermination)

        Call Me.modules.Clear()
        Call Me.moduleIndex.Clear()

        ' 二进制文件的结构为 
        '
        ' - string 模块名称字符串，最开始为长度整形数
        ' - integer 有多少个编号
        ' - string ZERO 使用零结尾的编号字符串
        '
        For Each [module] As PropertyInfo In modules
            Dim modAttr = [module].GetCustomAttribute(Of [Module])
            Dim modAttrEmpty = modAttr Is Nothing OrElse modAttr.Name.StringEmpty
            Dim name$ = modAttr.Name Or [module].Name.When(modAttrEmpty)
            Dim list$() = DirectCast([module].GetValue(Me), Index(Of String)).Objects

            Call stream.Write(name, BinaryStringFormat.DwordLengthPrefix)
            Call stream.Write(list.Length)

            For Each id As String In list
                Call stream.Write(id, BinaryStringFormat.ZeroTerminated)
            Next

            Call Me.modules.Add(name, [module].GetValue(Me))
            Call Me.moduleIndex.Add(name)
        Next

        Return Me
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Write(module$, time#, cycle As Dictionary(Of String, Double)) As Writer
        Return Write([module], time, cycle.Takes(modules([module]).Objects))
    End Function

    Public Function Write(module$, time#, data#()) As Writer

        ' 一个循环之后得到的结果数据的写入的结构为
        '
        ' - double time 时间值
        ' - byte 在header之中的module的索引号
        ' - double() data块，每一个值的顺序是和header之中的id排布顺序是一样的，长度和header之中的id列表保持一致
        '
        Call stream.Write(time)
        Call stream.Write(CByte(moduleIndex([module])))
        Call stream.Write(data)

        Return Me
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        Call stream.Flush()
        Call stream.Close()
        Call stream.Dispose()

        MyBase.Dispose(disposing)
    End Sub
End Class