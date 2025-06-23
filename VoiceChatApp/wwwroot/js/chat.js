console.log("✅ chat.js loaded");


let connection;
let peerConnection;
let localStream;

const config = {
    iceServers: [{ urls: 'stun:stun.l.google.com:19302' }]
};


function joinChat() {
    const username = document.getElementById('username').value;


    connection = new signalR.HubConnectionBuilder()
        .withUrl('/chatHub')
        .build();

    connection.start().then(() => {
        console.log("✅ SignalR connection started");
        connection.invoke('JoinRoom', username).then(() => {
            console.log("📡 JoinRoom invoked successfully");
        }).catch(err => {
            console.error("❌ Error invoking JoinRoom:", err);
        });
    }).catch(err => {
        console.error("❌ Error starting SignalR connection:", err);
    });

    connection.on('UserJoined', (id, name) => {
        console.log("🟢 UserJoined received:", id, name);
        if (!document.getElementById(id)) {
            const div = document.createElement('div');
            div.classList.add('user');
            div.id = id;
            div.innerHTML = `${name}
            <button onclick="call('${id}')">Call</button>
            <button onclick="endCall()">End Call</button>`;
            document.getElementById('users').appendChild(div);
        }
    });

    connection.on('UserLeft', (id) => {
        const div = document.getElementById(id);
        if (div) div.remove();
    });

    connection.on('ReceiveSignal', async (senderId, data) => {
        
    });
}









async function call(targetId) {
    await startLocalStream();
    peerConnection = createPeerConnection(targetId);
    const offer = await peerConnection.createOffer();
    await peerConnection.setLocalDescription(offer);
    connection.invoke('SendSignal', targetId, JSON.stringify(offer));
}





function endCall() {
    if (peerConnection) {
        peerConnection.ontrack = null;
        peerConnection.onicecandidate = null;
        peerConnection.close();
        peerConnection = null;
    }
    if (localStream) {
        localStream.getTracks().forEach(track => {
            track.stop();
        });
        localStream = null;
    }

    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(stream => {
            stream.getTracks().forEach(track => track.stop());
            stream = null;
        });

    document.getElementById('remoteAudio').srcObject = null;
}





function createPeerConnection(targetId) {
    const pc = new RTCPeerConnection(config);

    localStream.getTracks().forEach(track => {
        pc.addTrack(track, localStream);
    });

    pc.onicecandidate = e => {
        if (e.candidate) {
            connection.invoke('SendSignal', targetId, JSON.stringify(e.candidate));
        }
    };

    pc.ontrack = e => {
        document.getElementById('remoteAudio').srcObject = e.streams[0];
    };

    return pc;
}



async function startLocalStream() {
    try {
        localStream = await navigator.mediaDevices.getUserMedia({ audio: true, video: false });
    } catch (err) {
        console.error("❌ Failed to get user media:", err);
    }
}




