Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports [Module] = Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute

''' <summary>
''' The sandbox engine output raw data file format
''' </summary>
Public Class Raw : Implements IDisposable

    Public Const Magic$ = "GCModeller"

#Region "Cellular Modules"

    ''' <summary>
    ''' 由基因转录出来的mRNA的编号列表
    ''' </summary>
    ''' <returns></returns>
    <[Module]("Message-RNA")>
    Public ReadOnly Property mRNAId As Index(Of String)
    ''' <summary>
    ''' 由基因转录出来的其他的RNA分子的编号列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Component-RNA")>
    Public ReadOnly Property RNAId As Index(Of String)
    ''' <summary>
    ''' 由mRNA翻译出来的多肽链的Id列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Polypeptide")>
    Public ReadOnly Property Polypeptide As Index(Of String)
    ''' <summary>
    ''' 由一条或者多条多肽链修饰之后得到的最终的蛋白质的编号列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Protein")>
    Public ReadOnly Property Proteins As Index(Of String)
    ''' <summary>
    ''' 代谢物列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Metabolite")>
    Public ReadOnly Property Metabolites As Index(Of String)
    ''' <summary>
    ''' 反应过程编号列表
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <[Module]("Reaction-Flux")>
    Public ReadOnly Property Reactions As Index(Of String)

#End Region

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Function GetModules() As PropertyInfo()
        Return GetType(Raw) _
            .GetProperties(PublicProperty) _
            .Where(Function(prop)
                       Return prop.PropertyType Is GetType(Index(Of String))
                   End Function) _
            .ToArray
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class

Public Class Reader : Inherits Raw

    ReadOnly stream As BinaryDataReader

    Sub New(input As Stream)
        stream = New BinaryDataReader(input)
    End Sub

    Public Function LoadIndex() As Reader

    End Function

    Public Function Read(time#, module$) As Double()

    End Function
End Class

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