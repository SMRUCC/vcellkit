Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 反应过程通道
''' </summary>
Public Class Channel

    Dim left As Variable()
    Dim right As Variable()

End Class

Public Class Variable

    ''' <summary>
    ''' 对反应容器之中的某一种物质的引用
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Mass As Factor
    ''' <summary>
    ''' 在反应过程之中的变异系数，每完成一个单位的反应过程，当前的<see cref="Mass"/>将会丢失这个系数相对应的数量的含量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Coefficient As Double

    Public Overrides Function ToString() As String
        Return Mass.ToString
    End Function

End Class

''' <summary>
''' 一个变量因子，这个对象主要是用于存储值
''' </summary>
Public Class Factor : Inherits Value(Of Double)
    Implements INamedValue

    Public Property ID As String Implements IKeyedEntity(Of String).Key

    Public Overrides Function ToString() As String
        Return $"{ID} ({Value} unit)"
    End Function
End Class

''' <summary>
''' 一个反应容器，也是一个微环境，这在这个反应容器之中包含有所有的反应过程
''' </summary>
Public Class Vessel

    ''' <summary>
    ''' 当前的这个微环境之中的所有的反应过程列表
    ''' </summary>
    ''' <returns></returns>
    Public Property Channels As Channel()
    ''' <summary>
    ''' 当前的这个微环境之中的所有的物质列表
    ''' </summary>
    ''' <returns></returns>
    Public Property Mass As Factor()

End Class