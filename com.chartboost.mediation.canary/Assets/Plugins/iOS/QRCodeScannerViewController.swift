// Copyright 2022-2023 Chartboost, Inc.
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file.

import UIKit
import AVFoundation

@objc
public protocol QRCodeScannerViewControllerDelegate: AnyObject {
    func didScanQRCode(appId: String, appSignature: String)
}

@objc
public class QRCodeScannerViewController: UIViewController {

    @IBOutlet weak private var qrCodeScannerView: QRCodeScannerView!

    // MARK: - Lifecycle

    public override func viewDidLoad() {
        super.viewDidLoad()
        qrCodeScannerView.delegate = self
    }

    public override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        qrCodeScannerView.initialize()
    }

    public override var supportedInterfaceOrientations: UIInterfaceOrientationMask {
        [.portrait]
    }

    // MARK: - Properties

    @objc
    public weak var delegate: QRCodeScannerViewControllerDelegate?

    // MARK: - Actions

    @IBAction private func closeButtonPushed() {
        dismiss(animated: true)
    }

    // MARK: - Private

    private var scanCompleted = false

    private func presentAlert(error: LocalizedError) {
        let alert = UIAlertController(title: "Error", message: error.localizedDescription, preferredStyle: .alert)
        alert.addAction(.init(title: "OK", style: .cancel))
        present(alert, animated: true)
    }
}

extension QRCodeScannerViewController: QRCodeScannerViewDelegate {
    func didCompleteQRCodeScan(result: Result<URL, QRCodeScannerError>) {
        switch result {
        case .success(let url):
            guard
                let queryParameters = url.queryParameters,
                let appId = queryParameters["appId"],
                let appSignature = queryParameters["appSignature"] else {
                return presentAlert(error: QRCodeScannerError.scannedURLMissingData)
            }
            delegate?.didScanQRCode(appId: appId, appSignature: appSignature)
            dismiss(animated: true)
        case .failure(let error):
            presentAlert(error: error)
        }
    }

    public func metadataOutput(_ output: AVCaptureMetadataOutput, didOutput metadataObjects: [AVMetadataObject], from connection: AVCaptureConnection) {
        guard
            !scanCompleted,
            metadataObjects.count == 1,
            let readableObject = metadataObjects.first as? AVMetadataMachineReadableCodeObject,
            let stringValue = readableObject.stringValue,
            let url = URL(string: stringValue)
        else {
            return
        }
        guard stringValue.hasPrefix("helium://signin") else {
            return didCompleteQRCodeScan(result: .failure(.invalidScannedURL))
        }
        didCompleteQRCodeScan(result: .success(url))
        scanCompleted = true
    }
}

extension QRCodeScannerError: LocalizedError {
    var errorDescription: String? {
        switch self {
        case .unauthorized:
            return NSLocalizedString("Authorization has not been given to use the camera for video capture.", comment: "")
        case .scannedURLMissingData:
            return NSLocalizedString("Successfully scanned QR code but it is missing the required credentials.", comment: "")
        case .invalidScannedURL:
            return NSLocalizedString("The scanned QR code is not valid for this application.", comment: "")
        case .invalidCaptureDeviceInput, .failedToAddCaptureDeviceInput, .failedToAddCaptureDeviceOutput, .invalidCaptureDevice:
            return NSLocalizedString("Failed to initialize QR code capture view.", comment: "")
        }
    }
}
