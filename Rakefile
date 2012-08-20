require 'albacore'

MsBuildPath = "#{ENV['SystemRoot']}\\Microsoft.NET\\Framework\\v4.0.30319\\msbuild.exe"
@msTestPath = "C:\\Program Files (x86)\\Microsoft Visual Studio 10.0\\Common7\\IDE\\mstest.exe"
@aspNetCompilePath = "C:\\Windows\\Microsoft.NET\\Framework\\v2.0.50727\\aspnet_compiler.exe"

@defaultWorkingDirectory = pwd
@workspaceDir = nil
@solutionFile = nil
@iteration = nil

desc "Default local build task with tests."
task :default => ["build:base"]

namespace :build do
	task :base => [:setvariable, :msbuild, :aspnetcompile, :tests]
	
	task :setvariable do
		@solutionFile = File.expand_path("#{@defaultWorkingDirectory}\\UndeadEarth.sln")
    		@solutionFile.gsub!("/", "\\")	
	end
	
	desc "Build the TaxBuilder IDE solution."
	msbuild :msbuild do |msb|
	    msb.properties :configuration => :Debug, :TrackFileAccess => :false
	    msb.targets :Clean, :Build
            msb.path_to_command = MsBuildPath
	    msb.solution = "\"#{@solutionFile}\""
  	end
  	
  	task :tests do
  		cd "#{@defaultWorkingDirectory}\\UndeadEarth.Test\\bin\\Debug"
  		sh "\"#{@msTestPath}\" /testcontainer:UndeadEarth.Test.dll"
  	end
  	
  	task :aspnetcompile do
	  	sh "\"#{@aspNetCompilePath}\" -v temp -p \"#{@defaultWorkingDirectory}\\UndeadEarth.Web\""
  	end 
end
