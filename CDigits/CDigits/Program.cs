using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace CDigits
{
	public sealed class Program
	{
		// train d:\work_folder\machine_learning\1\unpacked\train-images.idx3-ubyte d:\work_folder\machine_learning\1\unpacked\train-labels.idx1-ubyte d:\GitHub\AI\CDigits\Network\netowrk.bin

		// test d:\work_folder\machine_learning\1\unpacked\t10k-images.idx3-ubyte d:\work_folder\machine_learning\1\unpacked\t10k-labels.idx1-ubyte d:\GitHub\AI\CDigits\Network\netowrk.bin

		private static readonly Double[] BipolarNegatives = new Double[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

		private static readonly Double[] UnipolarZeros = new Double[10];

		private static Record[] MergeData(Record[] records, Byte[] labels)
		{
			if (null == records)
			{
				throw new ArgumentOutOfRangeException("records");
			}

			if (null == labels)
			{
				throw new ArgumentOutOfRangeException("labels");
			}

			if (records.Length != labels.Length)
			{
				throw new ArgumentOutOfRangeException();
			}

			Double[][] outputs = new Double[10][];

			for (Int32 q = 0; q < outputs.Length; q++)
			{
				if (UseBipolar)
				{
					outputs[q] = (Double[])BipolarNegatives.Clone();
				}
				else
				{
					outputs[q] = (Double[])UnipolarZeros.Clone();
				}

				outputs[q][q] = 1;
			}

			Parallel.For(0, records.Length, x =>
				{
					records[x].Label = labels[x];
					records[x].Output = outputs[labels[x]];
				});

			return records;
		}

		private static Double[] ReduceBits(Byte[] input)
		{
			Double[] result = new Double[input.Length];

			for (Int32 q = 0; q < result.Length; q++)
			{
				Byte value = input[q];

				if (value != 0)
				{
					result[q] = 1;
				}
				else
				{
					if (UseBipolar)
					{
						result[q] = -1;
					}
					else
					{
						result[q] = 0;
					}
				}
			}

			return result;
		}

		private static Int32 ReadMsbFirstInt(BinaryReader reader)
		{
			Byte[] bytes = reader.ReadBytes(sizeof(Int32));

			Array.Reverse(bytes);

			return BitConverter.ToInt32(bytes, 0);
		}

		private sealed class Record
		{
			public Double[] Input { get; set; }

			public Double[] Output { get; set; }

			public Int32 Rows { get; set; }

			public Int32 Columns { get; set; }

			public Byte Label { get; set; }
		}

		private static Byte[] ReadLabels(String labelsPath)
		{
			Byte[] data = File.ReadAllBytes(labelsPath);
        
			using (MemoryStream buffer = new MemoryStream(data))
			{
				using (BinaryReader reader = new BinaryReader(buffer))
				{
					Int32 magicValue = ReadMsbFirstInt(reader);

					Int32 numberOfItems = ReadMsbFirstInt(reader);

					return reader.ReadBytes(numberOfItems);
				}
			}
		}

		private static Record[] ReadPixels(String pixelsPath)
		{
			Byte[] data = File.ReadAllBytes(pixelsPath);

			using (MemoryStream buffer = new MemoryStream(data))
			{
				using (BinaryReader reader = new BinaryReader(buffer))
				{
					Int32 magicValue = ReadMsbFirstInt(reader);

					Int32 numberOfItems = ReadMsbFirstInt(reader);

					Int32 numberOfRows = ReadMsbFirstInt(reader);

					Int32 numberOfColumns = ReadMsbFirstInt(reader);

					Int32 pixelsPeritem = numberOfRows * numberOfColumns;

					Int64 limit = buffer.Length - pixelsPeritem;

					List<Record> records = new List<Record>(numberOfItems);

					while (buffer.Position <= limit)
					{
						Byte[] bytes = reader.ReadBytes(pixelsPeritem);

						records.Add(new Record
							{
								Input = ReduceBits(bytes),
								Rows = numberOfRows,
								Columns = numberOfColumns
							});
					}

					return records.ToArray();
				}
			}
		}

		static void Main(String[] args)
		{
			try
			{
				String command = args[0];

				String inputFile = args[1];

				String inputLabelsFile = args[2];

				String networkPath = args[3];

				Record[] records = MergeData(ReadPixels(inputFile), ReadLabels(inputLabelsFile));

				switch (command)
				{
					case "train":
						LearningData learningData = PrepareLearningData(records);

						TrainNetwork(learningData, networkPath);
						break;

					case "test":
						TestNetwork(records, networkPath);
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private static Int32 DecodeResult(Double[] output)
		{
			Double maxValue = Double.MinValue;
			Int32 maxIndex = -1;

			for (Int32 q = 0; q < output.Length; q++)
			{
				if (output[q] > maxValue)
				{
					maxValue = output[q];
					maxIndex = q;
				}
			}

			return maxIndex;
		}

		private static void TestNetwork(Record[] inputData, String networkPath)
		{
			Network network = ActivationNetwork.Load(networkPath);

			Int32 correct = 0;

			Array.ForEach(inputData, record =>
				{
					Double[] output = network.Compute(record.Input);

					Int32 decodedResult = DecodeResult(output);

					if (decodedResult == record.Label)
					{
						correct++;
					}
					else
					{

					}
				});

			Console.WriteLine("{0} {1} {2}", correct, inputData.Length, (correct * 1.0) / inputData.Length);
		}

		private sealed class LearningData
		{
			internal Double[][] Input { get; set; }

			internal Double[][] Output { get; set; }
		}

		private static LearningData PrepareLearningData(Record[] inputData)
		{
			Double[][] input = new Double[inputData.Length][];

			Double[][] output = new Double[inputData.Length][];

			Parallel.For(0, inputData.Length, x =>
				{
					input[x] = inputData[x].Input;
					output[x] = inputData[x].Output;
				});

			return new LearningData
				{
					Input = input,
					Output = output
				};
		}

		private static readonly Boolean UseBipolar = false;

		private static void TrainNetwork(LearningData learningData, String networkPath)
		{
			ActivationNetwork network = new ActivationNetwork(
				UseBipolar ? (IActivationFunction)new BipolarSigmoidFunction(1) : (IActivationFunction)new SigmoidFunction(),
				784,
				//784,
				10);

			network.Randomize();

			Int32 epochIndex = 0;

			//// create teacher
			PerceptronLearning teacher = new PerceptronLearning(network);// new BackPropagationLearning(network);
			//PerceptronLearning teacher = new PerceptronLearning(network);// new BackPropagationLearning(network);

			teacher.LearningRate = 0.1f;
			////teacher.Momentum = 0.5f;

			Double error = Double.MaxValue;

			Double previousError = Double.MaxValue;

			Int32 counter = 100;
			// loop
			while (counter > 0)
			{
				// run epoch of learning procedure
				error = teacher.RunEpoch(learningData.Input, learningData.Output);

				if (error > previousError)
				{
					teacher.LearningRate = teacher.LearningRate * 0.5f;
				}

				Console.WriteLine(String.Format("{0} {1}", epochIndex, error));

				epochIndex++;

				previousError = error;

				counter--;
			}

			network.Save(networkPath);

			//Double[] output = network.Compute(learningData.Input[0]);			
		}
	}
}
