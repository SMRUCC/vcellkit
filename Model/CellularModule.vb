Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' A cellular system consist of two features:
''' 
''' + Genotype
''' + Phenotype
''' 
''' </summary>
Public Structure CellularModule

    Public Taxonomy As Taxonomy

    ''' <summary>
    ''' Genome
    ''' </summary>
    Public Genotype As Genotype
    ''' <summary>
    ''' Metabolome
    ''' </summary>
    Public Phenotype As Phenotype

End Structure
