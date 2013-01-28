/* $Id$
 * ======================================================================
 * 
 * Copyright 2010-2013 EMBL - European Bioinformatics Institute
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * ======================================================================
 * jDispatcher SOAP command-line client for MAFFT
 * ====================================================================== */
using System;
using System.IO;
using EbiWS.MafftWs;

namespace EbiWS
{
	class MafftCliClient : EbiWS.MafftClient
	{
		/// <summary>Tool specific usage</summary>
		private string usageMsg = @"MAFFT
=====

MAFFT (Multiple Alignment using Fast Fourier Transform) is a high speed 
multiple sequence alignment program.
    
[Required]

  seqFile            : file : sequences to align (""-"" for STDIN)

[Optional]

  -f, --format       : str  : alignment format, see --paramDetail format
  -m, --matrix       : str  : scoring matrix, see --paramDetail matrix
  -g, --gapopen      : real : gap creation penalty
  -x, --gapext       : real : gap extension penalty
  -r, --order        : str  : order of sequences in alignment, 
                              see --paramDetail order
      --nbtree       : int  : tree rebuilding number
      --maxiterate   : int  : maximum number of iterations
      --ffts         : str  : perform FFTS, see --paramDetail ffts
";

		/// <summary>Execution entry point</summary>
		/// <param name="args">Command-line parameters</param>
		/// <returns>Exit status</returns>
		public static int Main(string[] args)
		{
			int retVal = 0; // Return value
			// Create an instance of the wrapper object
			MafftCliClient wsApp = new MafftCliClient();
			// If no arguments print usage and return
			if (args.Length < 1)
			{
				wsApp.PrintUsageMessage();
				return retVal;
			}
			try
			{
				// Parse the command line
				wsApp.ParseCommand(args);
				// Perform the selected action
				switch (wsApp.Action)
				{
					case "paramList": // List parameter names
						wsApp.PrintParams();
						break;
					case "paramDetail": // Parameter detail
						wsApp.PrintParamDetail(wsApp.ParamName);
						break;
					case "submit": // Submit a job
						wsApp.SubmitJob();
						break;
					case "status": // Get job status
						wsApp.PrintStatus();
						break;
					case "resultTypes": // Get result types
						wsApp.PrintResultTypes();
						break;
					case "polljob": // Get job results
						wsApp.GetResults();
						break;
					case "help": // Do help
						wsApp.PrintUsageMessage();
						break;
					default: // Any other action.
						Console.WriteLine("Error: unknown action " + wsApp.Action);
						retVal = 1;
						break;
				}
			}
			catch (System.Exception ex)
			{ // Catch all exceptions
				Console.Error.WriteLine("Error: " + ex.Message);
				Console.Error.WriteLine(ex.StackTrace);
				retVal = 2;
			}
			return retVal;
		}

		/// <summary>
		/// Print the usage message.
		/// </summary>
		private void PrintUsageMessage()
		{
			PrintDebugMessage("PrintUsageMessage", "Begin", 1);
			Console.WriteLine(usageMsg);
			PrintGenericOptsUsage();
			PrintDebugMessage("PrintUsageMessage", "End", 1);
		}

		/// <summary>Parse command-line options</summary>
		/// <param name="args">Command-line options</param>
		private void ParseCommand(string[] args)
		{
			PrintDebugMessage("ParseCommand", "Begin", 1);
			InParams = new InputParameters();
			for (int i = 0; i < args.Length; i++)
			{
				PrintDebugMessage("parseCommand", "arg: " + args[i], 2);
				switch (args[i])
				{
						// Generic options
					case "--help": // Usage info
						Action = "help";
						break;
					case "-h":
						goto case "--help";
					case "/help":
						goto case "--help";
					case "/h":
						goto case "--help";
					case "--params": // List input parameters
						Action = "paramList";
						break;
					case "/params":
						goto case "--params";
					case "--paramDetail": // Parameter details
						ParamName = args[++i];
						Action = "paramDetail";
						break;
					case "/paramDetail":
						goto case "--paramDetail";
					case "--jobid": // Job Id to get status or results
						JobId = args[++i];
						break;
					case "/jobid":
						goto case "--jobid";
					case "--status": // Get job status
						Action = "status";
						break;
					case "/status":
						goto case "--status";
					case "--resultTypes": // Get result types
						Action = "resultTypes";
						break;
					case "/resultTypes":
						goto case "--resultTypes";
					case "--polljob": // Get results for job
						Action = "polljob";
						break;
					case "/polljob":
						goto case "--polljob";
					case "--outfile": // Base name for results file(s)
						OutFile = args[++i];
						break;
					case "/outfile":
						goto case "--outfile";
					case "--outformat": // Only save results of this format
						OutFormat = args[++i];
						break;
					case "/outformat":
						goto case "--outformat";
					case "--verbose": // Output level
						OutputLevel++;
						break;
					case "/verbose":
						goto case "--verbose";
					case "--quiet": // Output level
						OutputLevel--;
						break;
					case "/quiet":
						goto case "--quiet";
					case "--email": // User e-mail address
						Email = args[++i];
						break;
					case "/email":
						goto case "--email";
					case "--title": // Job title
						JobTitle = args[++i];
						break;
					case "/title":
						goto case "--title";
					case "--async": // Async submission
						Action = "submit";
						Async = true;
						break;
					case "/async":
						goto case "--async";
					case "--debugLevel":
						DebugLevel = Convert.ToInt32(args[++i]);
						break;
					case "/debugLevel":
						goto case "--debugLevel";
					case "--endpoint": // Service endpoint
						ServiceEndPoint = args[++i];
						break;
					case "/endpoint":
						goto case "--endpoint";

						// Tool specific options
				case "--format": // Output format
					InParams.format = args[++i];
					break;
				case "/format":
					goto case "--format";
				case "-f":
					goto case "--format";
				case "/f":
					goto case "--format";
				case "--matrix": // Scoring matrix
					InParams.matrix = args[++i];
					break;
				case "/matrix":
					goto case "--matrix";
				case "-m":
					goto case "--matrix";
				case "/m":
					goto case "--matrix";
				case "--gapopen": // Gap open penalty
					InParams.gapopen = Convert.ToSingle(args[++i]);
					InParams.gapopenSpecified = true;
					break;
				case "/gapopen":
					goto case "--gapopen";
				case "-g":
					goto case "--gapopen";
				case "/g":
					goto case "--gapopen";
				case "--gapext": // Gap extension penalty
					InParams.gapext = Convert.ToSingle(args[++i]);
					InParams.gapextSpecified = true;
					break;
				case "/gapext":
					goto case "--gapext";
				case "-x":
					goto case "--gapext";
				case "/x":
					goto case "--gapext";
				case "--order": // Order of sequences in alignment
					InParams.order = args[++i];
					break;
				case "/order":
					goto case "--order";
				case "-r":
					goto case "--order";
				case "/r":
					goto case "--order";
				case "--nbtree": // Tree rebuilding number
					InParams.nbtree = Convert.ToInt32(args[++i]);
					InParams.nbtreeSpecified = true;
					break;
				case "/nbtree":
					goto case "--nbtree";
				case "--maxiterate": // Max. number of iterations
					InParams.maxiterate = Convert.ToInt32(args[++i]);
					InParams.maxiterateSpecified = true;
					break;
				case "/maxiterate":
					goto case "--maxiterate";
				case "--ffts": // Perform FFTS
					InParams.ffts = args[++i];
					break;
				case "/ffts":
					goto case "--ffts";

						// Input data/sequence option
					case "--sequence": // Input sequence
						i++;
						goto default;
					default:
						// Check for unknown option
						if (args[i].StartsWith("--") || args[i].LastIndexOf('/') == 0)
						{
							Console.Error.WriteLine("Error: unknown option: " + args[i] + "\n");
							Action = "exit";
							return;
						}
						// Must be data argument
						InParams.sequence = LoadData(args[i]);
						Action = "submit";
						break;
				}
			}
			PrintDebugMessage("ParseCommand", "End", 1);
		}
	}
}