﻿#Region "Microsoft.VisualBasic::03c02a21094cd93c6f91a6f3cd02f56c, engine\Compiler\AssemblyScript\Commands\Command.vb"

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

    '     Class Command
    ' 
    '         Properties: commandName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace AssemblyScript.Commands

    ''' <summary>
    ''' the assembly compiler commands
    ''' </summary>
    Public MustInherit Class Command

        Public ReadOnly Property commandName As String
            Get
                Return MyClass.GetType.Name
            End Get
        End Property

        Public MustOverride Function Execute(env As Environment) As Object
        Public MustOverride Overrides Function ToString() As String

        Friend Shared Function stripValueString(text As String) As String
            Return Strings.Trim(text).Trim(""""c).Trim(" "c)
        End Function

    End Class
End Namespace
