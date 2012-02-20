require 'rake'
require 'albacore'
require 'fileutils'

PRODUCT_NAME = "Catnap"
BASE_PATH = File.expand_path ""
SOURCE_PATH = "#{BASE_PATH}/src/"
OUTPUT_PATH = "#{BASE_PATH}/output/"
UNIT_TESTS_PATH = "#{SOURCE_PATH}/#{PRODUCT_NAME}.UnitTests/bin/Release/"
INTEGRATION_TESTS_PATH = "#{SOURCE_PATH}/#{PRODUCT_NAME}.IntegrationTests/bin/Release/"
UNIT_TESTS_OUTPUT_PATH = "#{OUTPUT_PATH}/unit_tests"
PACKAGE_OUTPUT_PATH = "#{OUTPUT_PATH}/package/"
PACKAGE_OUTPUT_LIB_PATH = "#{PACKAGE_OUTPUT_PATH}/lib/"
INTEGRATION_TESTS_OUTPUT_PATH =  "#{OUTPUT_PATH}/integration_tests"
TEST_REPORT_PATH =  "#{OUTPUT_PATH}/report"
COMMON_ASSEMBY_INFO_FILE = "#{SOURCE_PATH}/CommonAssemblyInfo.cs"
VERSION = "0.1.0.0"
PACKAGE_PATH = "#{BASE_PATH}/package"
NUSPEC_FILENAME = "#{PRODUCT_NAME}.nuspec"
NUSPEC_PATH = "#{PACKAGE_PATH}/#{NUSPEC_FILENAME}"

task :default => [:full]
task :full => [:assemblyInfo,:build,:output,:test,:define_package]
task :package => [:full,:create_package]

msbuild :build do |msb|
	msb.properties :configuration => :Release
	msb.targets :Clean, :Build
	msb.verbosity = "minimal"
	msb.solution = "#{SOURCE_PATH}/#{PRODUCT_NAME}.sln"
end

task :output do
	FileUtils.rmtree OUTPUT_PATH
	FileUtils.mkdir OUTPUT_PATH
	FileUtils.mkdir TEST_REPORT_PATH
	FileUtils.mkdir UNIT_TESTS_OUTPUT_PATH
	FileUtils.mkdir INTEGRATION_TESTS_OUTPUT_PATH
	FileUtils.mkdir PACKAGE_OUTPUT_PATH
	FileUtils.mkdir PACKAGE_OUTPUT_LIB_PATH
	FileUtils.cp_r "#{UNIT_TESTS_PATH}.", UNIT_TESTS_OUTPUT_PATH
	FileUtils.cp_r "#{INTEGRATION_TESTS_PATH}.", INTEGRATION_TESTS_OUTPUT_PATH
	copy_files UNIT_TESTS_PATH, PACKAGE_OUTPUT_LIB_PATH, PRODUCT_NAME, ["dll", "pdb", "xml"]
end

mspec :test do |mspec|
	mspec.command = "#{SOURCE_PATH}/packages/Machine.Specifications.0.4.13.0/tools/mspec.exe"
	mspec.assemblies = "#{UNIT_TESTS_OUTPUT_PATH}/#{PRODUCT_NAME}.UnitTests.dll", "#{INTEGRATION_TESTS_OUTPUT_PATH}/#{PRODUCT_NAME}.IntegrationTests.dll"
	mspec.html_output = TEST_REPORT_PATH
end

desc "Assembly Version Info Generator"
assemblyinfo :assemblyInfo do |asm|
	FileUtils.rm COMMON_ASSEMBY_INFO_FILE, :force => true 
	asm.output_file = COMMON_ASSEMBY_INFO_FILE
	asm.title = PRODUCT_NAME
	asm.company_name = "Lunaverse Software"
	asm.product_name = PRODUCT_NAME
	asm.version = VERSION
	asm.file_version = VERSION
	asm.copyright = "Copyright (c) 2011 Lunaverse Software"
end

desc "Create the Catnap nuspec file"
nuspec :define_package do |nuspec|
	version = "#{ENV['version']}"
	nuspec.id = "Catnap"
	nuspec.version = version.length == 7 ? version : VERSION
	nuspec.authors = "Tim Scott"
	nuspec.owners = "Tim Scott"
	nuspec.description = "A basic lightweight object relational mapper (ORM) for .NET."
	nuspec.title = "Catnap"
	nuspec.language = "en-US"
	nuspec.licenseUrl = "https://github.com/timscott/catnap/blob/master/LICENSE.TXT"
	nuspec.projectUrl = "https://github.com/timscott/catnap/wiki"
	nuspec.tags = "ORM Sqlite Mobile MonoTouch Database ObjectRelationalMapping"
	nuspec.working_directory = PACKAGE_PATH
	nuspec.output_file = NUSPEC_FILENAME
end

desc "Create the nuget package"
task :create_package do
    cmd = Exec.new
    cmd.command = 'tools/nuget.exe'
    cmd.parameters = "pack #{NUSPEC_PATH} -basepath #{PACKAGE_OUTPUT_PATH} -outputdirectory #{PACKAGE_PATH}"
    cmd.execute
end

def copy_files(from, to, filename, extensions)
  extensions.each do |ext|
	from_name = "#{from}#{filename}.#{ext}"
	to_name = "#{to}#{filename}.#{ext}"
	if File.exists? from_name
		puts "Copying '#{from_name}' to '#{to_name}'"
		FileUtils.cp from_name, to_name
	end
  end
end