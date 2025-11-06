#!/bin/sh

#  run-test-case.sh
#  Contentstack Delivery SDK
#
#  Created based on Management SDK pattern
#  Copyright © 2025 Contentstack. All rights reserved.

echo "Removing old test results..."

TEST_TARGETS=('Contentstack.Core.Unit.Tests' 'Contentstack.Core.Tests')

for i in "${TEST_TARGETS[@]}"
do
   rm -rf "$i/TestResults"
done

DATE=$(date +'%d-%b-%Y')

FILE_NAME="Contentstack-Delivery-DotNet-Test-Case-$DATE"

echo "Running all test cases (unit + integration) with coverage..."
dotnet test Contentstack.Net.sln \
  --logger "trx;LogFileName=Report-$FILE_NAME.trx" \
  --collect:"XPlat code coverage" \
  --verbosity minimal

echo "Test case Completed..."

echo "Generating HTML coverage report..."

# Restore local tools if needed
dotnet tool restore > /dev/null 2>&1

# Use reportgenerator from local tools - output to root TestResults folder
dotnet tool run reportgenerator "-reports:**/**/coverage.cobertura.xml" "-targetdir:TestResults/Coverage-$FILE_NAME" "-reporttypes:HTML;HTMLSummary" "-classfilters:-*Tests*;-*Mokes*;-*Mocks*" "-filefilters:-*Tests*;-*Mokes*;-*Mocks*"

echo "HTML coverage report generated at: TestResults/Coverage-$FILE_NAME/index.html"
echo "Report generation completed!"

