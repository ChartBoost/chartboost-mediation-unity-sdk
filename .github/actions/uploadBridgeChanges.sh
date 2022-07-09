git add "com.chartboost.helium/Runtime/Plugins/Android/helium-android-bridge.jar"

if ! git commit -m 'Android Bridge Changes Found! Updating Bridge'
then
  echo 'No Android Bridge Changes Found, Nothing to Commit!'
else
  echo "Android Bridge Changes Found, Attempting Push!"
  echo "Head Ref: ${GITHUB_HEAD_REF}"
  if git push "origin/${GITHUB_HEAD_REF}" HEAD:"${GITHUB_HEAD_REF}"
  then
    echo "Successfuly Pushed Android Bridge Changes"
  else
    echo "Failed to Push Android Bridge Changes"
    exit 1
  fi
fi
