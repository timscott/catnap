require 'rake'
require 'albacore'
require 'fileutils'

PRODUCT_NAME = "Catnap"
SOURCE_PATH = File.expand_path "src"
OUTPUT_PATH = File.expand_path "output"
UNIT_TESTS_PATH = "#{SOURCE_PATH}/#{PRODUCT_NAME}.UnitTests"
INTEGRATION_TESTS_PATH = "#{SOURCE_PATH}/#{PRODUCT_NAME}.IntegrationTests"
UNIT_TESTS_OUTPUT_PATH = "#{OUTPUT_PATH}/unit_tests"
INTEGRATION_TESTS_OUTPUT_PATH =  "#{OUTPUT_PATH}/integration_tests"
TEST_REPORT_PATH =  "#{OUTPUT_PATH}/report"
COMMON_ASSEMBY_INFO_FILE = "#{SOURCE_PATH}/CommonAssemblyInfo.cs"

task :default => [:full]

task :full => [:assemblyInfo,:build,:copy_for_test,:test]

msbuild :build do |msb|
	msb.properties :configuration => :Release
	msb.targets :Clean, :Build
	msb.verbosity = "minimal"
	msb.solution = "#{SOURCE_PATH}/#{PRODUCT_NAME}.sln"
end

task :copy_for_test do
	FileUtils.rmtree OUTPUT_PATH
	FileUtils.mkdir OUTPUT_PATH
	FileUtils.mkdir TEST_REPORT_PATH
	FileUtils.mkdir UNIT_TESTS_OUTPUT_PATH
	FileUtils.mkdir INTEGRATION_TESTS_OUTPUT_PATH
	FileUtils.cp_r "#{UNIT_TESTS_PATH}/bin/Release/.", UNIT_TESTS_OUTPUT_PATH
	FileUtils.cp_r "#{INTEGRATION_TESTS_PATH}/bin/Release/.", INTEGRATION_TESTS_OUTPUT_PATH
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
	asm.version = "0.1.0.0"
	asm.file_version = "0.1.0.0"
	asm.copyright = "Copyright {c) 2011 Lunaverse Software"
end