#!/usr/bin/env python3
"""Build the solution first, then verify both samples using only Python's stdlib."""

import json
import os
from pathlib import Path
import socket
import subprocess
import tempfile
import time
import urllib.error
import urllib.request


ROOT = Path(__file__).resolve().parents[1]
NAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
EMAIL = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
ROLE = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
HTTP = urllib.request.build_opener(urllib.request.ProxyHandler({}))


def request(url, host=None):
    headers = {"Host": host} if host else {}
    try:
        with HTTP.open(urllib.request.Request(url, headers=headers), timeout=2) as response:
            return response.status, response.read()
    except urllib.error.HTTPError as error:
        return error.code, error.read()


def check_sample(version, profile, expected_name, expected_email, manager):
    project = ROOT / "Samples" / f"FakeAuth.SampleWeb{version}.0" / f"FakeAuth.SampleWeb{version}.0.csproj"
    assert project.is_file(), f"Missing sample project: {project}"
    assembly = project.parent / "bin" / "Release" / f"net{version}.0" / f"{project.stem}.dll"
    assert assembly.is_file(), f"Build the Release solution first: missing {assembly}"
    with socket.socket() as listener:
        listener.bind(("127.0.0.1", 0))
        port = listener.getsockname()[1]
    url = f"http://localhost:{port}"
    with tempfile.TemporaryFile(mode="w+") as log:
        process = subprocess.Popen(
            # Launch the app itself so cleanup owns one process on every OS.
            ["dotnet", str(assembly), "--urls", url, "--profile", profile],
            cwd=project.parent,
            env={**os.environ, "ASPNETCORE_ENVIRONMENT": "Development"},
            stdout=log, stderr=subprocess.STDOUT,
        )
        try:
            deadline = time.monotonic() + 30
            while True:
                assert process.poll() is None, f"Sample exited with code {process.returncode}"
                try:
                    status, body = request(url + "/api/claims")
                    break
                except (urllib.error.URLError, TimeoutError):
                    if time.monotonic() >= deadline:
                        raise AssertionError("Sample did not become ready within 30 seconds")
                    time.sleep(0.1)
            assert status == 200, f"Claims endpoint returned {status}"
            claims = {claim["type"]: claim["value"] for claim in json.loads(body)}
            assert claims[NAME] == expected_name, claims
            assert claims[EMAIL] == expected_email, claims
            assert (claims.get(ROLE) == "Manager") == manager, claims
            status, body = request(url + "/api/manager")
            assert status == (200 if manager else 403), f"Manager endpoint returned {status}"
            if manager:
                assert json.loads(body)["name"] == expected_name, body
            status, _ = request(url + "/api/claims", host="example.com")
            assert status == 401, f"Unexpected host accepted: HTTP {status}"
            print(f"PASS .NET {version}: {profile} claims, role authorization, host restriction", flush=True)
        except BaseException:
            log.seek(0)
            print(log.read(), flush=True)
            raise
        finally:
            if process.poll() is None:
                process.terminate()
                try:
                    process.wait(timeout=5)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait()


if __name__ == "__main__":
    for version in (10, 11):
        for case in (
            ("default", "Fake User", "fake@fakeuser.com", False),
            ("manager", "Manager User", "manager@example.test", True),
            ("inline", "Inline User", "inline@example.test", True),
        ):
            check_sample(version, *case)
    print("All 6 sample configurations passed.")
