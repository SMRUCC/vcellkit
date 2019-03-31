Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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

    Public Sub Initialize()

    End Sub

    ''' <summary>
    ''' 当前的这个微环境的迭代器
    ''' </summary>
    Public Iterator Function ContainerIterator() As IEnumerable(Of NamedValue(Of Double))
        For Each reaction As Channel In Channels
            ' 不可以使用Where直接在for循环外进行筛选
            ' 因为环境是不断地变化的
            Yield iterateFlux(reaction)
        Next
    End Function

    Private Function iterateFlux(reaction As Channel) As NamedValue(Of Double)
        Dim regulate#

        Select Case reaction.Direction
            Case Directions.LeftToRight
                ' 消耗左边，产生右边
                regulate = reaction.Forward.Coefficient

                If regulate > 0 Then
                    ' 当前是具有调控效应的
                    ' 接着计算最小的反应单位
                    regulate = reaction.CoverLeft(regulate)
                End If
                If regulate > 0 Then
                    ' 当前的过程是可以进行的
                    ' 则进行物质的转义的计算
                    Call reaction.Transition(regulate, Directions.LeftToRight)
                End If
            Case Directions.RightToLeft
                regulate = reaction.Reverse.Coefficient

                If regulate > 0 Then
                    regulate = reaction.CoverRight(regulate)
                End If
                If regulate > 0 Then
                    Call reaction.Transition(regulate, Directions.RightToLeft)
                End If
            Case Else
                ' no reaction will be happends
                regulate = 0
        End Select

        Return New NamedValue(Of Double)(reaction.ID, regulate)
    End Function
End Class