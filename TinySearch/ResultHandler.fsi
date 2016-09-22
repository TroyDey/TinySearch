namespace TinySearch

module ResultHandler =

    open SearchTypes

    val outputResults : pagination -> scoredResult list -> outputResult list
