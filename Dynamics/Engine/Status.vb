#Region "Microsoft.VisualBasic::2dd849c9dd029a3166ba449ae1e4ea02, engine\Dynamics\Status.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:

' Module Status
' 
' 
' 
' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace Core

    ''' <summary>
    ''' 细胞状态定义
    ''' </summary>
    ''' <remarks>
    ''' 在这里应该是通过各种代谢物分子之间的浓度相对百分比来定义诸如死亡或者细胞分裂之类的状态？
    ''' </remarks>
    Public Class Status

        ''' <summary>
        ''' 状态名称
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        ''' <summary>
        ''' 当虚拟细胞反应容器<see cref="Vessel"/>中的<see cref="Vessel.Mass"/>浓度
        ''' 百分比接近于这个向量的百分比的时候就认为<see cref="name"/>状态或者细胞活动事件发生了
        ''' </summary>
        ''' <returns></returns>
        Public Property definition As Dictionary(Of String, Double)

        Public Function IsCurrentStatus(cell As Vessel, cutoff#) As Boolean
            Dim current As Double() = cell.Mass.Take
        End Function

    End Class
End Namespace
