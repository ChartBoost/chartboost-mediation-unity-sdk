// Copyright 2022-2023 Chartboost, Inc.
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file.

import UIKit
import AVFoundation

enum QRCodeScannerError: Error {
    case failedToAddCaptureDeviceInput
    case failedToAddCaptureDeviceOutput
    case invalidCaptureDevice
    case invalidCaptureDeviceInput
    case invalidScannedURL
    case scannedURLMissingData
    case unauthorized
}

protocol QRCodeScannerViewDelegate: AVCaptureMetadataOutputObjectsDelegate {
    func didCompleteQRCodeScan(result: Result<URL, QRCodeScannerError>)
}

class QRCodeScannerView: UIView {

    // MARK: - Lifecycle

    deinit {
        captureSession?.stopRunning()
    }

    // MARK: - Properties

    weak var delegate: QRCodeScannerViewDelegate?

    // MARK: - Methods

    func initialize() {
        let authorizationStatus = AVCaptureDevice.authorizationStatus(for: .video)
        switch authorizationStatus {
        case .notDetermined:
            requestAuthorization()
        case .authorized:
            prepare()
        default:
            emitNoAccessError()
        }
    }

    // MARK: - Private

    private var captureSession: AVCaptureSession?
    private var captureDevice: AVCaptureDevice?
    private var videoPreviewLayer: AVCaptureVideoPreviewLayer?

    private func prepare() {
        guard let captureDevice = AVCaptureDevice.default(for: .video) else {
            delegate?.didCompleteQRCodeScan(result: .failure(.invalidCaptureDevice))
            return
        }

        guard let captureDeviceInput = try? AVCaptureDeviceInput(device: captureDevice) else {
            delegate?.didCompleteQRCodeScan(result: .failure(.invalidCaptureDeviceInput))
            return
        }

        let captureSession = AVCaptureSession()
        guard captureSession.canAddInput(captureDeviceInput) else {
            delegate?.didCompleteQRCodeScan(result: .failure(.failedToAddCaptureDeviceInput))
            return
        }
        captureSession.addInput(captureDeviceInput)

        let captureMetadataOutput = AVCaptureMetadataOutput()
        guard captureSession.canAddOutput(captureMetadataOutput) else {
            delegate?.didCompleteQRCodeScan(result: .failure(.failedToAddCaptureDeviceOutput))
            return
        }
        captureSession.addOutput(captureMetadataOutput)
        captureMetadataOutput.setMetadataObjectsDelegate(delegate, queue: .main)
        captureMetadataOutput.metadataObjectTypes = [.qr]

        let videoPreviewLayer = AVCaptureVideoPreviewLayer(session: captureSession)
        videoPreviewLayer.frame = layer.bounds
        layer.addSublayer(videoPreviewLayer)

        self.captureSession = captureSession
        self.captureDevice = captureDevice

        captureSession.commitConfiguration()
        DispatchQueue.global(qos: .background).async {
            captureSession.startRunning()
        }
    }

    private func requestAuthorization() {
        AVCaptureDevice.requestAccess(for: .video) { [weak self] success in
            guard let self = self else { return }
            DispatchQueue.main.async {
                if success {
                    self.prepare()
                } else {
                    self.emitNoAccessError()
                }
            }
        }
    }

    private func emitNoAccessError() {
        delegate?.didCompleteQRCodeScan(result: .failure(.unauthorized))
    }
}
