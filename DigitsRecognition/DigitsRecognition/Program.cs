using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Neuro;
using Accord.Neuro.Learning;

namespace DigitsRecognition
{
	/*
		0 314494,738677974 250,4440888
		1 117666,265848241 249,5486972
		2 523496,099333732 254,1381827
		3 105114,571823427 264,6639819
		4 99958,0598963173 260,0906556
		5 213952,391715191 269,4658629
		6 90972,1880365019 277,3694028
		7 64073,6322054075 276,1276809
		8 74648,995370101 281,2433476
		9 58501,7934474445 280,8170517
		10 49919,0895917476 285,4433435
		11 40848,8248992452 276,115259
		12 36385,4373772446 271,8165549
		13 32240,3635141704 260,2436114
		14 29640,6502607366 249,180458
		15 26701,4513840967 249,328713
		16 24687,2937691624 251,6906092
		17 23108,1749788113 251,1978169
		18 21290,3432581243 256,4317232
		19 19983,2868375255 278,6484773
		20 18659,4600279096 261,5108409
		21 17457,9876684204 288,2304132
		22 16414,6045644959 264,998976
		23 15448,4958562642 258,6811559
		24 14550,9038700535 254,9254525
		25 13704,3920579475 269,5866113
		26 12937,3275811556 263,9765967
		27 12239,2866203262 255,1822576
		28 11585,9141065775 266,3247158
		29 10973,9191556309 291,753586
		30 10381,4536970946 297,9051989
		31 9850,02273407669 20757,8829861
		32 9331,2760840058 266,947681
		33 8882,97771852419 254,6361818
		34 8460,93239298489 256,4521774
		35 8066,49600987722 258,1904155
		36 7681,42632777438 251,9906347
		37 7306,42560555458 252,0908191
		38 6956,31301361967 250,9434419
		39 6645,10828242251 250,8646442
		40 6340,53029253135 250,7191332
		41 6049,74978373902 250,7744177
		42 5770,24823989563 250,6791041
		43 5501,8551231557 964,1143977
		44 5257,47034165036 249,937879
		45 5023,17057949735 249,5924563
		46 4791,35184043493 249,5782567
		47 4587,00138657869 249,9632656
		48 4389,39519233825 249,6238812
		49 4196,74054818679 250,4962531
		50 4025,37295200451 266,374909
		51 3868,32700351807 251,9832777
		52 3703,93851100546 249,5199301
		53 3549,6617424855 256,8607485
		54 3403,55404057365 261,2926225
		55 3259,14948628777 253,7173501
		56 3124,49036938522 250,6768903
		57 3006,04805811462 250,8483917
		58 2886,79658214025 249,8611401
		59 2785,32733101175 249,965657
		60 2684,79076734834 250,0981945
		61 2584,76096894927 249,9888918
		62 2489,1207301351 249,9058964
		63 2392,08266635055 249,7989456
		64 2295,3052363024 249,7289639
		65 2201,7047955935 249,8872084
		66 2112,8015993016 250,6462146
		67 2024,92147055946 250,2918696
		68 1955,44524575487 251,4151831
		69 1892,5064691034 250,0128827
		70 1828,95338744546 250,2541736
		71 1771,2500521608 250,5211145
		72 1716,15592402224 250,0455364
		73 1662,6820663357 250,8143132
		74 1609,86231589548 250,5563299
		75 1556,2167750797 249,5710815
		76 1503,71590220931 249,4017384
		77 1453,26764444261 249,4260344
		78 1404,06116455786 251,4775058
		79 1360,26074600929 256,2975159
		80 1316,86516280772 250,195896
		81 1279,35545198996 253,1668625
		82 1240,84652676249 251,5589505
		83 1203,37270028258 249,8634116
		84 1169,30902538615 250,6604069
		85 1137,97523557633 251,3207328
		86 1110,06948948078 263,4651494
		87 1079,27348055895 258,9830978
		88 1050,4618193551 257,5674414
		89 1021,93403623589 251,05475
		90 996,324736529278 250,7807086
		91 971,311435049394 250,0573944
		92 946,393521507629 249,8046689
		93 922,116725326137 249,7652883
		94 900,451817054838 250,3245359
		95 879,880031838715 250,7516965
		96 861,578730128453 251,2942628
		97 841,249998534307 250,2260022
		98 823,121629376173 251,0124077
		99 806,31058318063 251,861405
		100 791,390174784357 251,3648486
		101 776,427456323582 252,1233422
		102 762,029608460228 252,6197979
		103 747,866703773511 252,8424127
		104 733,757262185674 252,8627199
		105 720,360605965317 252,9260941
		106 706,74497224214 252,1976469
		107 694,13717154085 251,8026952
		108 682,403263478857 252,0402411
		109 671,601231892579 249,9268467
		110 660,462552600823 249,903765
		111 649,911635810929 250,0704992
		112 639,293722663808 249,7024844
		113 628,673230951343 249,8102452
		114 619,119854242819 249,7333653
		115 612,181861156556 249,8030194
		116 601,179104965682 249,7736758
		117 592,380462954478 249,8272511
		118 583,13127587993 249,7466562
		119 574,481279837871 249,6769007
		120 568,026937877185 249,7104126
		121 561,69492802749 249,8370443
		122 555,392350892652 249,7542977
		123 549,192675198311 249,6673881
		124 543,972679852349 249,7430483
		125 538,463837055449 250,178082
		126 532,062899403649 249,9060059
		127 525,389122329964 249,8521607
		128 518,988968962745 249,8571574
		129 512,76439096171 249,9091111
		130 506,166399592362 250,1483456
		131 498,409535666203 250,1934338
		132 492,162502775826 250,4619637
		133 487,508599887529 258,6400756
		134 482,18648065473 250,1912867
		135 475,747590484835 249,733912
		136 470,840090889701 249,8288278
		137 463,805681284957 250,2498078
		138 456,510070754957 258,8716747
		139 450,758327133274 266,0349248
		140 445,70504594171 258,4808358
		141 440,971262509689 257,6736319
		142 435,403804441023 251,4481817
		143 430,837013235791 251,9620893
		144 427,282195531306 250,3529933
		145 424,777468596921 250,5298722
		146 422,71437357831 251,5496622
		147 420,97937749536 251,8701596
		148 418,989221381114 251,6814859
		149 416,097903596422 251,3797786
		150 413,161981947481 251,1565008
		151 410,006156361558 251,2039299
		152 407,029471723038 250,8863379
		153 403,039326480499 251,3428372
		154 399,243150303999 252,0061711
		155 395,939773630891 251,4996742
		156 394,072207139136 250,8060447
		157 391,410481871147 252,3157571
		158 388,987535196595 253,1615676
		159 387,201875282159 251,4337352
		160 384,888078236935 265,4880187
		161 382,044277504009 266,6507615
		162 379,527030083453 256,7355232
		163 376,472344208955 252,9784965
		164 374,15112187394 253,2137518
		165 371,962350817439 253,4417978
		166 369,570999383725 254,0871021
		167 368,251946138389 256,2124904
		168 367,425126184969 251,2445558
		169 365,989652926299 250,76832
		170 364,777560516006 250,6693285
		171 364,19243197627 251,3028636
		172 363,245516168732 251,293676
		173 362,071878701154 258,6401755
		174 361,594535573999 265,9538058
		175 361,2193438588 260,0348322
		176 360,631669010416 261,265289
		177 359,194376181671 252,1554386
		178 356,658000418292 253,6905192
		179 352,917122458752 288,5980655
		180 349,272135627505 283,3783072
		181 346,189993844784 289,5278935
		182 343,057759814897 293,4770843
		183 341,589159983941 302,3479568
		184 340,022444796371 283,6775223
		185 337,683467435966 274,292198
		186 334,442933298396 283,0673281
		187 332,652445229183 271,9966405
		188 331,301066942303 277,9386726
		189 329,113536760865 287,2638041
		190 326,422105777204 305,1063114
		191 325,23400663896 287,8524248
		192 323,710355137686 281,5513959
		193 320,735928868553 289,19708
		194 318,302719119615 273,5135844
		195 316,302056354144 269,88638
		196 314,854356829625 288,0039719
		197 313,571608456352 285,8964577
		198 312,870787201251 288,1594572
		199 312,326874060506 293,4161348
		
	    Result:
	   
	    9716 10000 0,9716
	 
	 */


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

		private static readonly Boolean UseBipolar = true;

		private static void TrainNetwork(LearningData learningData, String networkPath)
		{
			ActivationNetwork network = new ActivationNetwork(
				UseBipolar ? (IActivationFunction)new BipolarSigmoidFunction(1) : (IActivationFunction)new SigmoidFunction(),
				784,
				784,
				10);

			network.Randomize();

			Int32 epochIndex = 0;

			new NguyenWidrow(network).Randomize();

			//// create teacher
			//PerceptronLearning teacher = new PerceptronLearning(network);// new BackPropagationLearning(network);
			//PerceptronLearning teacher = new PerceptronLearning(network);// new BackPropagationLearning(network);
			ParallelResilientBackpropagationLearning teacher = new ParallelResilientBackpropagationLearning(network);

			//teacher.LearningRate = 0.0125;
			////teacher.Momentum = 0.5f;

			Double error = Double.MaxValue;

			Double previousError = Double.MaxValue;

			Stopwatch sw = new Stopwatch();

			Int32 counter = 100;
			// loop
			while (counter > 0)
			{
				sw.Reset();

				sw.Start();

				// run epoch of learning procedure
				error = teacher.RunEpoch(learningData.Input, learningData.Output);

				sw.Stop();

				//if (error > previousError)
				//{
				//	teacher.LearningRate = teacher.LearningRate * 0.5f;
				//}

				Console.WriteLine(String.Format("{0} {1} {2}", epochIndex, error, sw.Elapsed.TotalSeconds));

				epochIndex++;

				previousError = error;

				counter--;
			}

			network.Save(networkPath);

			//Double[] output = network.Compute(learningData.Input[0]);			
		}
	}
}
