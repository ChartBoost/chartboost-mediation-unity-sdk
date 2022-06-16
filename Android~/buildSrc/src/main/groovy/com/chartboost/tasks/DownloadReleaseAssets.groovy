package com.chartboost.tasks

import com.chartboost.GitHub.GitHub;
import org.gradle.api.DefaultTask;
import org.gradle.api.tasks.Input
import org.gradle.api.tasks.Internal
import org.gradle.api.tasks.OutputDirectory
import org.gradle.api.tasks.TaskAction;

public class DownloadReleaseAssets extends DefaultTask {
    @Input
    def String releaseTag;

    @Input
    def String repo

    @OutputDirectory
    def File directory

    @Internal
    GitHub gh = new GitHub();

    DownloadReleaseAssets() {
        outputs.upToDateWhen { false }
    }

    @TaskAction
    def download() {
        def release = gh.getReleaseByTagName(repo, releaseTag)

        print("Downloading assets for tag $releaseTag in $repo into $directory\n")

        for(def asset : release.assets) {
            def name = asset.name

            print("  - $name (${asset.size} bytes)\n")
            def file = new File(directory, name)
            gh.downloadReleaseAsset(asset.url, file)
            if (file.size() != asset.size) {
                throw new RuntimeException("File size mismatch for $name expected ${asset.size} actual ${file.size()}")
            }
        }
    }
}
