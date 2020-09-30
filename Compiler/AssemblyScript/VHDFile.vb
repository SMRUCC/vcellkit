Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Commands

Namespace AssemblyScript

    Public Class VHDFile

        ''' <summary>
        ''' build from a base model
        ''' </summary>
        ''' <returns></returns>
        Public Property base As From
        Public Property metadata As Label()
        Public Property maintainer As Maintainer
        Public Property keywords As Keywords
        Public Property environment As Env()
        Public Property modifications As Modification()

        Public Shared Function Parse(script As String) As VHDFile

        End Function

    End Class
End Namespace