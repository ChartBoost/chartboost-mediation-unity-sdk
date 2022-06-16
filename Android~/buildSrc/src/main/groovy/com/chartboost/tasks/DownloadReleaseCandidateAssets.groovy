package com.chartboost.tasks

import com.chartboost.GitHub.GitHub
import org.gradle.api.DefaultTask
import org.gradle.api.tasks.Input
import org.gradle.api.tasks.Internal
import org.gradle.api.tasks.OutputDirectory
import org.gradle.api.tasks.TaskAction

class DownloadReleaseCandidateAssets extends DefaultTask {
    @Input
    def String commit

    @Input
    def String releaseTag

    @Input
    def String repo

    @OutputDirectory
    def File directory

    @Internal
    GitHub gh = new GitHub()

    DownloadReleaseCandidateAssets() {
        outputs.upToDateWhen { false }
    }

    @TaskAction
    def download() {
        def releaseCandidate = getReleaseCandidate()

        print("Downloading assets from tag ${releaseCandidate.tag_name} in $repo into $directory\n")

        for(def asset : releaseCandidate.assets) {
            def name = asset.name

            print("  - $name (${asset.size} bytes)\n")
            def file = new File(directory, name)
            gh.downloadReleaseAsset(asset.url, file)
            if (file.size() != asset.size) {
                throw new RuntimeException("File size mismatch for $name expected ${asset.size} actual ${file.size()}")
            }
        }
    }

    private def getReleaseCandidate() {
        def releases = gh.listReleases(repo)

        def releaseCandidate = releases.find { release ->
            def tagNamesMatch = release.tag_name.startsWith("${releaseTag}-rc")
            def hasAssets = release.assets != null && release.assets.size > 0

            tagNamesMatch &&
                    hasAssets &&
                    gh.getCommitHashByTagName(repo, release.tag_name) == commit
        }

        if (releaseCandidate == null) {
            throw new RuntimeException("No release candidate found for commit=$commit and releaseTag=$releaseTag in repo $repo\n")
        }

        releaseCandidate
    }
}
