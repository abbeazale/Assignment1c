import React, { useState, useRef } from 'react';
import './App.css'; // Import your CSS file

const VideoPlayer = () => {
    const [videoSource, setVideoSource] = useState('');
    const [youtubeUrl, setYoutubeUrl] = useState('');
    const videoRef = useRef(null);
    const fileInputRef = useRef(null);

    // Handle YouTube URL submission
    const handleYoutubeSubmit = (e) => {
        e.preventDefault();
        const videoId = getYoutubeId(youtubeUrl);
        if (videoId) {
            const embedUrl = `https://www.youtube.com/embed/${videoId}`;
            console.log("Generated YouTube Embed URL:", embedUrl); // Debugging
            setVideoSource(embedUrl);
        } else {
            console.error("Invalid YouTube URL");
            alert("Invalid YouTube URL. Please check the link and try again.");
        }
    };

    // Extract YouTube ID from various URL formats
    const getYoutubeId = (url) => {
        const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/;
        const match = url.match(regExp);
        return match && match[2].length === 11 ? match[2] : null;
    };

    // Handle local file selection
    const handleFileSelect = async (e) => {
        const file = e.target.files[0];
        if (file && file.type.startsWith('video/')) {
            await handleUpload(file);
        } else {
            alert("Invalid file type. Please upload a video file.");
        }
    };

    const handleUpload = async (file) => {
        const formData = new FormData();
        formData.append('file', file);

        try {
            console.log("Attempting to fetch...");
            const response = await fetch('https://localhost:7172/api/video/upload', {
                method: 'POST',
                body: formData,
                credentials: 'include'
            });
            console.log("Fetch completed, response received");
            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`HTTP error! Status: ${response.status} - ${errorText}`);
            }

            const data = await response.json();
            console.log("Video URL:", data.fileUrl); // Add this line
            setVideoSource(data.fileUrl);
        } catch (error) {
            console.error('Upload failed:', error);
            console.error('Error details:', {
                message: error.message,
                stack: error.stack,
                type: error.type
            });
            alert(error.message);
        }
    };

    // Toggle between URL and file input
    const showFilePicker = () => {
        fileInputRef.current.click();
    };

    return (
        <div className="video-player-container">
            <div className="input-section">
                <form onSubmit={handleYoutubeSubmit}>
                    <input
                        type="text"
                        value={youtubeUrl}
                        onChange={(e) => setYoutubeUrl(e.target.value)}
                        placeholder="Enter YouTube URL"
                    />
                    <button type="submit">Load YouTube Video</button>
                </form>
                <button onClick={showFilePicker}>Upload Local Video</button>
                <input
                    type="file"
                    ref={fileInputRef}
                    onChange={handleFileSelect}
                    accept="*"
                    hidden
                />
            </div>

            <div className="video-container">
                {videoSource.includes('youtube.com') ? (
                    <iframe
                        width="640"
                        height="360"
                        src={videoSource}
                        frameBorder="0"
                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                        allowFullScreen
                        title="YouTube Video Player"
                    ></iframe>
                ) : (
                    videoSource && (
                        <video ref={videoRef} width="640" height="360" controls>
                            <source src={videoSource} type="video/mp4" />
                            Your browser does not support the video tag.
                        </video>
                    )
                )}
            </div>
        </div>
    );
};

export default VideoPlayer;