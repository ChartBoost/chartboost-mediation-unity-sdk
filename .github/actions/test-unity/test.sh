echo $SETUP_PASS | sudo -S $UNITY_EXE_PATH -runTests -batchmode -nographics -projectPath $PROJECT_PATH -testResults $TESTS_PATH -testPlatform EditMode -logfile -
