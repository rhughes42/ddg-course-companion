// Program.cs
// Main entry point for DDG Companion examples
// Added by Graph Technologies, 2025

using System;
using DDGCompanion.Examples;

namespace DDGCompanion
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║  DDG Course Companion - C# Implementation    ║");
            Console.WriteLine("║  Added by Graph Technologies, 2025           ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝\n");
            
            if (args.Length == 0)
            {
                ShowMenu();
                return;
            }
            
            // Run specific example
            switch (args[0].ToLower())
            {
                case "smoothing":
                case "1":
                    LaplacianSmoothing.Run();
                    break;
                    
                case "curvature":
                case "2":
                    CurvatureAnalysis.Run();
                    break;
                    
                case "geodesic":
                case "3":
                    GeodesicDistance.Run();
                    break;
                    
                case "conformal":
                case "4":
                    ConformalMapping.Run();
                    break;
                    
                default:
                    Console.WriteLine($"Unknown example: {args[0]}");
                    ShowMenu();
                    break;
            }
        }
        
        static void ShowMenu()
        {
            Console.WriteLine("Available Examples:");
            Console.WriteLine("==================\n");
            Console.WriteLine("  1. smoothing  - Mesh smoothing via mean curvature flow");
            Console.WriteLine("  2. curvature  - Discrete Gaussian curvature computation");
            Console.WriteLine("  3. geodesic   - Geodesic distance via heat method");
            Console.WriteLine("  4. conformal  - Conformal surface parameterization\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run [example_name]\n");
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run smoothing");
            Console.WriteLine("  dotnet run curvature");
        }
    }
}
