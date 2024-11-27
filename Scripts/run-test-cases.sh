#!/bin/sh

#  run-test-case.sh
#  Contentstack
#
#  Copyright © 2024 Contentstack. All rights reserved.

echo "Removing files"

TEST_TARGETS=('Contentstack.Core.Tests')

for i in "${TEST_TARGETS[@]}"
do
   rm -rf "$i/TestResults"
done

DATE=$(date +'%d-%b-%Y')

FILE_NAME="Contentstack-DotNet-Test-Case-$DATE"

echo "Running test case..."
dotnet test --logger "trx;LogFileName=Report-$FILE_NAME.trx" --collect:"XPlat code coverage"

echo "Test case Completed..."

echo "Generating code coverage report..."

for i in "${TEST_TARGETS[@]}"
do
    cd "$i"
    reportgenerator "-reports:**/**/coverage.cobertura.xml" "-targetdir:TestResults/Coverage-$FILE_NAME" -reporttypes:HTML
    cd ..
done

echo "Code coverage report generate."
