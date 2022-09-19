using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace InsertSamplesIntoMarkDown
{
    class Program
    {
        private static readonly Regex RegionsRegex = new Regex(@"(\s*#region\s+)([^\r\n]+)((.|\r|\n)+?(?=#endregion))", RegexOptions.Compiled);
        static void Main(string[] args)
        {
            string sourceFile = (args.Length > 1 ? args[1] : "Samples.cs");
            string targetFile = (args.Length > 2 ? args[2] : "README.md");
            string timeout  = (args.Length > 4 ? args[4] : "");
            int timeoutSeconds = string.IsNullOrEmpty(timeout) ? 30 : int.Parse(timeout);
            Console.WriteLine($"Updating {targetFile} from {sourceFile}...");
            string baseFolder = Directory.GetCurrentDirectory();
            string sourceFileFullPath = Path.Combine(baseFolder, sourceFile);
            string targetFileFullPath = Path.Combine(baseFolder, targetFile);
            for (DateTime startTime = DateTime.UtcNow; (DateTime.UtcNow - startTime) < TimeSpan.FromSeconds(timeoutSeconds); )
            {
                try
                {
                    string sourceFileContents = File.ReadAllText(sourceFileFullPath);
                    MatchCollection rawRegions = RegionsRegex.Matches(sourceFileContents);
                    string targetFileContents = File.ReadAllText(targetFileFullPath);
                    foreach (Match match in rawRegions.Cast<Match>())
                    {
                        string regionName = match.Groups[2].Value;
                        string regionContents = match.Groups[3].Value.Trim();
                        string targetInsert = $"[//]: # ({regionName})\r\n```csharp\r\n{regionContents}\r\n```";
                        string targetReplacePattern = @"\[\/\/\]\:\s*\#\s*\(" + regionName + @"\).*[\r\n]```[^\r\n]*(.|\r|\n)+?(?=```)```";
                        targetFileContents = Regex.Replace(targetFileContents, targetReplacePattern, targetInsert);
                    }
                    targetFileContents = targetFileContents.TrimEnd();
                    File.WriteAllText(targetFileFullPath, targetFileContents);
                    Console.WriteLine($"Updated {targetFile} from {sourceFile}!");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating {targetFile} from {sourceFile}: {ex.Message}\r\nRetrying in a bit...");
                    System.Threading.Thread.Sleep(250 + (int)(GetRandom() % 250));
                    // fall through and retry
                }
            }
        }
        private static readonly long _startTickCount = Environment.TickCount;
        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();  // use this stopwatch to get seeds whose numbers change much faster than the tick count (which is generally every 15ms)
        private static long _rotator;      // interlocked
        static ulong GetRandom()
        {
            return (ulong)((_startTickCount + _stopwatch.ElapsedTicks) ^ System.Threading.Interlocked.Increment(ref _rotator));   // note that, yes, the stopwatch ticks are in different units than the environment ticks they're being added to, but we don't care about that here
        }
    }
}
/* Source Powershell:
# NOTE: you may have to run this the first time you build:
# Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
"Updating README.md from Samples.cs..."
$scriptPath=If(!$PSCommandPath) { (Get - Location).Path }
Else { Split-Path -parent $PSCommandPath }
$samplesFile=$scriptPath+"\AmbientServicesSamples\Samples.cs"
$readmeFile=$scriptPath+"\README.md"
$samplesFileContents=Get-Content $samplesFile -Raw
$samplesRegionPattern="[\r\n]\s*#region\s+([^\r\n\s]*)(.|\r|\n)+?(?=#endregion)"
$rawRegions=[Regex]::Matches($samplesFileContents,"(\s*#region\s+)([^\r\n]+)((.|\r|\n)+?(?=#endregion))")
$readmeFileContents=Get-Content $readmeFile -Raw
ForEach($_ in $rawRegions)
{ 
    $regionName =$_.Groups[2].Value
    $regionContents =$_.Groups[3].Value.Trim()
    $sampleInsert = "[//]: # (" + $regionName + ")`r`n``````csharp`r`n" + $regionContents + "`r`n``````"
    $readmeReplacePattern = "\[\/\/\]\:\s*\#\s*\(" + $regionName + "\).*[\r\n]``````[^\r\n]*(.|\r|\n)+?(?=``````)``````"
    $readmeFileContents =[Regex]::Replace($readmeFileContents,$readmeReplacePattern,$sampleInsert)
    #($readmeFileContents -Replace $readmeReplacePattern,$sampleInsert)
}
$readmeFileContents=$readmeFileContents.TrimEnd();
Set-Content -Path $readmeFile -Value $readmeFileContents
"Updated README.md from Samples.cs!"
*/
