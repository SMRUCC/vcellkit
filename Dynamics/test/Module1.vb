Imports Dynamics

Module Module1

    Sub Main()
        Dim massTable = mass.ToDictionary(Function(m) m.ID)
        Dim envir As New Vessel With {
            .Mass = massTable.Values.ToArray,
            .Channels = reactions(massTable).ToArray
        }

        For i As Integer = 0 To 100
            Call envir.ContainerIterator()
        Next

        Pause()
    End Sub

    Private Iterator Function mass() As IEnumerable(Of Factor)
        Yield New Factor With {.ID = "A", .Value = 100}
        Yield New Factor With {.ID = "B", .Value = 100}
        Yield New Factor With {.ID = "C", .Value = 100}
        Yield New Factor With {.ID = "D", .Value = 100}
        Yield New Factor With {.ID = "E", .Value = 100}
        Yield New Factor With {.ID = "F", .Value = 100}
        Yield New Factor With {.ID = "G", .Value = 100}
    End Function

    ''' <summary>
    ''' Build a test network
    ''' </summary>
    ''' <param name="massTable"></param>
    ''' <returns></returns>
    Private Iterator Function reactions(massTable As Dictionary(Of String, Factor)) As IEnumerable(Of Channel)
        Dim pop = Iterator Function(names As String()) As IEnumerable(Of Variable)
                      For Each ref In names
                          Yield New Variable(massTable(ref), 0.05)
                      Next
                  End Function

        Yield New Channel(pop({"A", "B"}), pop({"C", "D"})) With {
            .Forward = New Regulation,
            .Reverse = New Regulation With {.Activation = pop({"B"}).ToArray}}

        Yield New Channel(pop({"E", "F"}), pop({"A", "G"})) With {
            .Forward = New Regulation,
            .Reverse = New Regulation With {.Activation = pop({"B"}).ToArray}
        }

        Yield New Channel(pop({"B"}), pop({"A", "D"})) With {
            .Forward = New Regulation With {.Activation = pop({"C", "G"}).ToArray},
            .Reverse = New Regulation With {.Activation = pop({"E"}).ToArray}
        }

        Yield New Channel(pop({"G"}), pop({"E"})) With {
            .Forward = New Regulation With {.Activation = pop({"F"}).ToArray}
        }
    End Function
End Module
