
namespace FDigits

open System.IO
open System.Collections.Generic

module Idx =

    let readMsbFirstInt (reader:BinaryReader) = 
        let bytes = reader.ReadBytes(4)
        let reversed = Array.rev bytes
        System.BitConverter.ToInt32(reversed, 0)

    let readLabels path = 
        let data = File.ReadAllBytes path
        use buffer = new MemoryStream(data)
        use reader = new BinaryReader(buffer)
        let magicValue = readMsbFirstInt reader
        let numberOfItems = readMsbFirstInt reader
        reader.ReadBytes(numberOfItems)

    let reducer (input: byte) =
        if input <> byte 0 then byte 1
        else byte 0

//        match input with
//        | 0 -> byte 0
//        | _ -> byte 1

    let reduceBits pixels =
        pixels |> Array.map reducer

    let readData path =
        let data = File.ReadAllBytes path
        use buffer = new MemoryStream(data)
        use reader = new BinaryReader(buffer)
        let magicValue = readMsbFirstInt reader
        let numberOfItems = readMsbFirstInt reader
        let numberOfRows = readMsbFirstInt reader
        let numberOfColumns = readMsbFirstInt reader
        let pixelsPerItem = numberOfRows * numberOfColumns 
        let limit = buffer.Length - int64 pixelsPerItem

        let records = new List<byte[]>()
        while buffer.Position <= limit do
            let bytes = reader.ReadBytes(pixelsPerItem)
            records.Add(reduceBits bytes)
        records.ToArray()

    let mergeData labels pixels =
        Array.zip labels pixels

    let printData (input: byte * byte[]) =
        let label, pixels = input

        let mutable index = 0

        for i in [0..27] do
            System.Console.WriteLine()
            for j in [0..27] do
                let value = pixels.[index]
                System.Console.Write(value)
                index <- index + 1
            
    let readAllData labelsPath pixelspath =
        let labels = readLabels labelsPath
        let pixels = readData pixelspath
        mergeData labels pixels
        
    let distance pixels1 pixels2 =
        Array.zip pixels1 pixels2
        |> Array.map (fun (x,y) -> abs (int32 x - int32 y))
        |> Array.sum

    let train (input: (byte * byte[])[]) =
        let classify (pixels: byte[]) =
            input
            |> Array.minBy (fun (itemLabel, itemPixels) -> distance itemPixels pixels)
            |> fun (itemLabel, itemPixels) -> itemLabel
        classify

    let verifyModel (testData: (byte * byte[])[]) classifier =
        testData
        |> Array.averageBy (fun (itemLabel, itemData) -> if classifier itemData = itemLabel then 1. else 0.)
        |> printfn "Correct: %.3f"