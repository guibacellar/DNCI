# Convert Binary Data into Base64 Data

$InputFile = "..\DNCI.Injector.Library\resources\DNCIClrLoader.bin";
$OutputFile = "..\DNCI.Injector.Library\resources\DNCIClrLoader.bin";
$b64 = [System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes($InputFile))
[System.IO.File]::WriteAllText($OutputFile, $b64)
