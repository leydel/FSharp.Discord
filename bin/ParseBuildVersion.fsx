open System
open System.Text.RegularExpressions

type Version = {
  Major: int
  Minor: int
  Patch: int
}

module Version =
  let tryParseTagRef (str: string) =
    let pattern = @"^refs/tags/v(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)$"
    let matched = Regex.Match(str, pattern)

    match matched.Success with
    | false -> None
    | true -> Some {
      Major = int matched.Groups["major"].Value
      Minor = int matched.Groups["minor"].Value
      Patch = int matched.Groups["patch"].Value
    }

  let toString (version: Version) =
    $"{version.Major}.{version.Minor}.{version.Patch}"

let input =
  fsi.CommandLineArgs
  |> Seq.skip 1
  |> Seq.tryHead
  |> Option.defaultValue ""

match Version.tryParseTagRef input with
| Some version -> Console.WriteLine (Version.toString version)
| None -> failwith $"Unable to parse version '{input}'"
