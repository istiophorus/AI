// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Idx.fs"
open FDigits

// Define your library scripting code here

let allData = Idx.readAllData @"d:\work_folder\machine_learning\1\unpacked\train-labels.idx1-ubyte" @"d:\work_folder\machine_learning\1\unpacked\train-images.idx3-ubyte"

let allTestData = Idx.readAllData @"d:\work_folder\machine_learning\1\unpacked\t10k-labels.idx1-ubyte" @"d:\work_folder\machine_learning\1\unpacked\t10k-images.idx3-ubyte"

let classifier = Idx.train allData

Idx.verifyModel allTestData classifier
