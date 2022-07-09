SUCCESS_PUSH="Successfuly Pushed Android Bridge Changes"
FAILED_PUSH="Failed to Push Android Bridge Changes"

git add "com.chartboost.helium/Runtime/Plugins/Android/helium-android-bridge.jar"

if ! git commit -m 'Android Bridge Changes Found! Updating Bridge'
then
  echo 'No Android Bridge Changes Found, Nothing to Commit!'
else
  echo "Android Bridge Changes Found, Attempting Push!"

  # if we are on a pull request, find the HEAD_REF and force push our bridge changes
  if [ -z "$GITHUB_HEAD_REF" ];
  then
    if git push -f origin HEAD:"${GITHUB_HEAD_REF}"
    then
      echo $SUCCESS_PUSH
    else
      echo $FAILED_PUSH
      exit 1
    fi
  # if we are on a branch, no need to do anything
  else
    if git push
    then
      echo $SUCCESS_PUSH
    else
      echo $FAILED_PUSH
      exit 1
    fi
  fi
fi
