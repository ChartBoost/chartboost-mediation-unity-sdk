# Unit Testing
The Helium Unity SDK package contains a series of Unit tests to ensure functionality and callbacks behave as expected.

In order to enable test from the Helium SDK package, the following setup is needed in your Unity's project `manifest.json`, as follows:

```json
  "dependencies": {
    ...
  },
  "testables": ["com.chartboost.mediation"]
```

Please refer to the following [Unity Documentation](https://docs.unity3d.com/Manual/cus-tests.html) for more information on Test in Packages and how to enabled them.
