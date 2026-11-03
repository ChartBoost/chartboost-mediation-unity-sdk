@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.banner

import android.app.Activity
import android.os.Looper
import android.view.View
import android.view.ViewGroup
import android.widget.FrameLayout
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdView
import com.chartboost.unity.utilities.view.UnityViewUtils
import org.junit.After
import org.junit.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.Shadows.shadowOf
import org.robolectric.annotation.Config

// xhdpi: density 2, so dp → px conversions are visible.
@RunWith(RobolectricTestRunner::class)
@Config(sdk = [34], qualifiers = "xhdpi")
class BannerAdWrapperTest {

    private lateinit var activity: Activity
    private lateinit var root: ViewGroup
    private lateinit var ad: ChartboostMediationBannerAdView
    private lateinit var wrapper: BannerAdWrapper
    private val defaultActivityProvider = activityProviderField.get(UnityViewUtils)

    @Before
    fun setUp() {
        activity = Robolectric.buildActivity(Activity::class.java).setup().get()
        setActivityProvider { activity }
        root = activity.findViewById(android.R.id.content)
        ad = ChartboostMediationBannerAdView(activity)
        root.addView(ad)
        wrapper = BannerAdWrapper(ad)
        wrapper.setListener(NoOpListener)
    }

    @After
    fun tearDown() = setActivityProvider(defaultActivityProvider)

    @Test
    fun fixedSizeCentersTheCreativeByDefault() {
        val creative = fixedCreative()

        assertEquals((640 - 200) / 2f, creative.translationX)
        assertEquals((200 - 50) / 2f, creative.translationY)
    }

    @Test
    fun aPinnedCreativeIgnoresAlignmentReSets() {
        val creative = pinnedCreative()

        wrapper.setHorizontalAlignment(1) // already center: a per-frame re-set
        wrapper.setContainerSize(320, 100) // a later layout pass must not move a still-pinned creative
        settle()

        assertEquals(20f, creative.x)
        assertEquals(10f, creative.y)
    }

    @Test
    fun anAlignmentChangeHandsPlacementBack() {
        val creative = pinnedCreative()

        wrapper.setHorizontalAlignment(2) // right
        settle()

        assertEquals(640f - 200f, creative.translationX)
    }

    @Test
    fun aNewCreativeStartsFromTheAlignment() {
        pinnedCreative()

        val refreshed = addCreative() // a refresh
        settle()

        assertEquals((640 - 200) / 2f, refreshed.translationX)
    }

    @Test
    @Config(qualifiers = "420dpi") // density 2.625: 30 dp is 78.75 px
    fun containerSizesRoundToTheNearestPixel() {
        wrapper.setContainerSize(30, 30)

        assertEquals(79, ad.layoutParams.width)
        assertEquals(79, ad.layoutParams.height)
    }

    // A 200x50 px creative, as the SDK adds one on load and on each refresh.
    private fun addCreative(): View {
        val creative = View(activity).apply { layoutParams = FrameLayout.LayoutParams(200, 50) }
        ad.removeAllViews()
        ad.addView(creative)
        ad.chartboostMediationBannerAdViewListener?.onAdViewAdded("placement", creative)
        return creative
    }

    // A creative in a 320x100 dp (640x200 px) fixed-size container.
    private fun fixedCreative(): View = addCreative().also {
        wrapper.setContainerSize(320, 100)
        settle()
    }

    // A fixed-size creative pinned at 10,5 dp, as BannerVisualElement places it.
    private fun pinnedCreative(): View = fixedCreative().also {
        wrapper.setAdRelativePosition(10f, 5f)
        settle()
    }

    // Lays out from the root and drains posted partner updates; the second pass covers the relayout they trigger.
    private fun settle() = repeat(2) {
        root.measure(
            View.MeasureSpec.makeMeasureSpec(1080, View.MeasureSpec.EXACTLY),
            View.MeasureSpec.makeMeasureSpec(1920, View.MeasureSpec.EXACTLY),
        )
        root.layout(0, 0, 1080, 1920)
        shadowOf(Looper.getMainLooper()).idle()
    }

    // UnityPlayer can't load on the JVM; utilities' activity seam is internal to that module, so set it reflectively.
    private fun setActivityProvider(provider: Any?) = activityProviderField.set(UnityViewUtils, provider)

    private companion object {
        val activityProviderField = UnityViewUtils::class.java.getDeclaredField("activityProvider").apply { isAccessible = true }
    }

    private object NoOpListener : ChartboostMediationBannerAdListener {
        override fun onAdViewAdded(ad: BannerAdWrapper) = Unit
        override fun onAdClicked(ad: BannerAdWrapper) = Unit
        override fun onAdImpressionRecorded(ad: BannerAdWrapper) = Unit
    }
}
