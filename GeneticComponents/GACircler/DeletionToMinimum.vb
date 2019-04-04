Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Helper
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

''' <summary>
''' 通过删减基因的方法将基因组最小化
''' </summary>
Public Module DeletionToMinimum

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model">这个数据模型之中仅包含有生物学功能的描述，并没有任何序列信息</param>
    ''' <param name="define"></param>
    ''' <param name="popSize">进化的种群大小</param>
    ''' <param name="eval">计算突变体的对环境的适应度</param>
    ''' <returns>
    ''' 这个函数返回的是目标可能的最优解下的所有的剩余的遗传元件的编号列表，然后下游程序可以根据这个编号列表来进行全基因组序列的装配
    ''' </returns>
    Public Function DoDeletion(model As CellularModule, define As Definition, eval As Func(Of Vessel, Double), Optional popSize% = 500) As String()
        Dim envir As Vessel = New Loader(define).CreateEnvironment(model)
        Dim population As Population(Of Genome) = New Genome().InitialPopulation(5000)
        Dim ga As New GeneticAlgorithm(Of Genome)(population, New Fitness(eval))
        Dim engine As New EnvironmentDriver(Of Genome)(ga) With {
            .Iterations = 10000,
            .Threshold = 0.005
        }

        Call engine.AttachReporter(Sub(i, e, g) EnvironmentDriver(Of Genome).CreateReport(i, e, g).ToString.__DEBUG_ECHO)
        Call engine.Train()

        Dim solution = ga.Best.chromosome
    End Function


End Module

''' <summary>
''' 基因组是由遗传元件所构成的
''' </summary>
Public Class Genome : Implements Chromosome(Of Genome)

    ''' <summary>
    ''' 染色体上面的基因以及调控位点的构成，1表示存在，0表示缺失
    ''' </summary>
    Friend chromosome As Integer()

    Shared ReadOnly random As New Random()

    Private Function Clone() As Genome
        Return New Genome With {
            .chromosome = chromosome.ToArray
        }
    End Function

    Public Iterator Function Crossover(another As Genome) As IEnumerable(Of Genome) Implements Chromosome(Of Genome).Crossover
        Dim thisClone As Genome = Me.Clone()
        Dim otherClone As Genome = another.Clone()

        Call random.Crossover(thisClone.chromosome, another.chromosome)

        Yield thisClone
        Yield otherClone
    End Function

    Public Function Mutate() As Genome Implements Chromosome(Of Genome).Mutate
        Dim result As Genome = Me.Clone()
        Call result.chromosome.ByteMutate(random)
        Return result
    End Function
End Class

Public Class Fitness : Implements Fitness(Of Genome)

    Public ReadOnly Property Cacheable As Boolean Implements Fitness(Of Genome).Cacheable
        Get
            Return False
        End Get
    End Property

    Sub New(target As Func(Of Vessel, Double))

    End Sub

    Public Function Calculate(chromosome As Genome) As Double Implements Fitness(Of Genome).Calculate

    End Function
End Class