Public Class Metadata

    ''' <summary>
    ''' 总时间大小
    ''' </summary>
    ''' <returns></returns>
    Public Property total_time As Double
    ''' <summary>
    ''' 每一帧时间的数据快照对应的时间点，例如：
    ''' 
    ''' frame_1.zip -> [0] 0.0min
    ''' frame_2.zip -> [1] 1.0min
    ''' frame_3.zip -> [2] 2.0min
    ''' </summary>
    ''' <returns></returns>
    Public Property time_frames As Double()

    ''' <summary>
    ''' 使用一个一维数组来表示一个三维空间的三维数组 boolean(,,)，<see cref="width"/>, <see cref="height"/>, <see cref="depth"/>标记了这个三维数组的维度信息。
    ''' 在这个三维空间中，采用boolean来标记模拟的空间形状，false表示空（不存在任何数据），true表示对应的位置是模拟环境空间的一部分
    ''' </summary>
    ''' <returns></returns>
    Public Property shape As Boolean()

    ''' <summary>
    ''' width of the <see cref="shape"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property width As Integer
    ''' <summary>
    ''' height of the <see cref="shape"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property height As Integer
    ''' <summary>
    ''' depth of the <see cref="shape"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property depth As Integer

End Class
