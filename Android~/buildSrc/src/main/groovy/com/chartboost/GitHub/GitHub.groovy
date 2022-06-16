package com.chartboost.GitHub

import groovy.json.JsonSlurper

class GitHub {
    private final String token = System.getenv("GITHUB_TOKEN")

    public def listReleases(String repo) {
        // Heads up!  The output of most GitHub api calls is paginated with a page size of 30
        // by default.  The max is 100.  This method doesn't deal with pagination other than
        // to ask for the max # of results possible.  If we ever have more than 100 releases
        // (which include release candidates until they're deleted). then we'll have to do
        // something here.
        return queryAndParseJson("$repo/releases?per_page=100");
    }

    public def getReleaseByTagName(String repo, String tag) {
        return queryAndParseJson("$repo/releases/tags/$tag");
    }

    public def getCommitHashByTagName(String repo, String tag) {
        "https://api.github.com/repos/ChartBoost/$repo/commits/refs/tags/$tag".toURL().getText(
                requestProperties: [
                        Authorization: "token $token",
                        Accept: "application/vnd.github.v3.sha"])
    }

    public void downloadReleaseAsset(String url, File outputFile) {
        // We pass access_token as a query parameter here,
        // rather than in a header as in the other queries here.
        // The reason is that the response is sometimes a 302 redirect,
        // with the URL in the location: header in the response.  This location
        // URL also has authentication information.
        // If both were present, the response would be an error like this:
        //     Only one auth mechanism allowed; only the X-Amz-Algorithm query parameter,
        //     Signature query string parameter or the Authorization header should be specified

        execOrDie(
                "curl",
                "--fail",   // return nonzero exit code on 404 etc
                "--show-error",
                "-H", "Authorization: token $token",
                "-H", "Accept:application/octet-stream",
                "-L",       // follow redirects
                "-o", outputFile.absolutePath,
                "$url")
    }

    private static def execOrDie(String... cmd) {
        def proc = cmd.execute();

        proc.consumeProcessOutput(System.out, System.err);
        proc.waitFor();
        if (proc.exitValue() != 0) {
            throw new RuntimeException("Failed with exit code ${proc.exitValue()}.\n")
        }
    }

    private def queryAndParseJson(String partialUrl) {
        String s = query(partialUrl)
        return new JsonSlurper().parseText(s)
    }

    private String query(String partialUrl) {
        "https://api.github.com/repos/ChartBoost/$partialUrl".toURL().getText(
                requestProperties: [Authorization: "token $token"])
    }
}
