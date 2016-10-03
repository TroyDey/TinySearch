namespace TinySearch

module ResultHandler =

    open SearchTypes

    val outputResults : pagination -> scoredResult seq -> outputResult seq
