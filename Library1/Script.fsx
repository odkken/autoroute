#r @"..\routing\bin\debug\routing.exe"

let numElements = 18

let sequence = [1 .. numElements]

let rand = new System.Random()

let swap (a: _[]) x y =
    let tmp = a.[x]
    a.[x] <- a.[y]
    a.[y] <- tmp

// shuffle an array (in-place)
let shuffle a =
    Array.iteri (fun i _ -> swap a i (rand.Next(i, Array.length a))) a

let solution = sequence |> Seq.toArray

shuffle solution

System.Console.WriteLine(System.String.Join(",", solution))

let evaluateFitness (values: int list) =
    let mutable fitness = 0
    for i = 0 to (values |> Seq.length) - 1 do
        if not(values.[i] = solution.[i]) then
            fitness <- fitness + 1
    fitness


let tree = new Routing.TreeLevel(new System.Collections.Generic.List<int>(sequence), 0)

while (tree.GetCurrentValues() |> Seq.length) < (solution |> Seq.length) do
    tree.WalkDown()
let mutable bestSolution = tree.GetCurrentValues() |> Seq.toList
let mutable bestFitness = evaluateFitness bestSolution

let mutable i = 0

while true do
    i <- i + 1
    let newVals = tree.GetCurrentValues() |> Seq.toList
    let fitness = evaluateFitness newVals
    if (newVals |> Seq.length) < (solution |> Seq.length) then
        if fitness < bestFitness then
            tree.WalkDown()
        else
            tree.WalkOver((newVals |> Seq.length) - 1)
    else
        if fitness < bestFitness then
            bestFitness <- fitness
            bestSolution <- newVals
            System.Console.WriteLine("new best solution:")
            tree.PrintValues()
        tree.WalkOver((solution |> Seq.length) - 1)

i

evaluateFitness(tree.GetCurrentValues() |> Seq.toList)


tree.PrintValues()
tree.WalkOver(4)
tree.WalkDown()





