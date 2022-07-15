# checks if directory exist and removes it with its contents
deleteDirectory () {
  local directory=$1
  echo "[cleanup-unity] Target Directory: $1"
  if [ -d $directory ]
  then
    echo "[cleanup-unity] Directory $directory Exists, Debugging Contents" && ls $directory
    echo $SETUP_PASS | sudo -S rm -d -rf $directory && echo "[cleanup-unity] Deleting Directory $directory"
  else
    echo "[cleanup-unity] Directory $directory Does No Exist"
  fi
}

# delete test retults directory
deleteDirectory "$PROJECT_PATH/test-results"
# deletes Library directory
deleteDirectory "$PROJECT_PATH/Library"
# deletes Logs directory
deleteDirectory "$PROJECT_PATH/Logs"
# deletes UserSettings directory
deleteDirectory "$PROJECT_PATH/UserSettings"
