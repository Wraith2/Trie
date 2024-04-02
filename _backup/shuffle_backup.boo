import System
import System.IO


def NumberFromFileName(fileName as string):
	match = /(?<name>.*\.old)(?<number>\d{2})/.Match(fileName)
	if match==null or not match.Success:
		raise InvalidOperationException(fileName)
	return match.Groups['name'].ToString(),int.Parse(match.Groups['number'].ToString())


def Main(args as (string)):
	//if not args or args.Length<3:
	//	print("shuffle_backup <directory> <backupName> <backupSubDir>")
	//	return

	//directory,backupName,backupSubDir = args

	backupSubDir=Path.GetFileName(Environment.CurrentDirectory)
	directory=Path.GetDirectoryName(Environment.CurrentDirectory)
	backupName=Path.GetFileName(directory)+'.zip'

	print "backupSubDir='${backupSubDir}'"
	print "backupName  ='${backupName}'"
	print "directory   ='${directory}'"
	

	files = [file for file in Directory.GetFiles(Path.Combine(directory,backupSubDir)) if file=~/old\d{2}/]
	files.Sort()

	for file in reversed(files):
		name,number as int = NumberFromFileName(file)
		File.Move(file,"${name}${(number+1).ToString('D2')}")
		# print "move \"${file}\" \"${name}${(number+1).ToString('D2')}\""

	backupFile = Path.Combine(Path.Combine(directory,backupSubDir),backupName)
	if File.Exists(backupFile):
		File.Move(backupFile,"${backupFile}.old01")
		#print "move \"${backupFile}\" \"${backupFile}.old01\""

	print "done"