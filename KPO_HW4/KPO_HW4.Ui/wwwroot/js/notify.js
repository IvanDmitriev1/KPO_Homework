export function isSupported() {
    return typeof window !== "undefined" && "Notification" in window;
}

export function currentPermission() {
    if (!isSupported()) {
        return "unsupported";
    }

    return Notification.permission;
}

export async function requestPermission() {
    if (!isSupported()) {
        return "unsupported";
    }
    return await Notification.requestPermission();
}

export function show(title, body, options = {}) {
    if (!isSupported())
        return { ok: false, reason: "unsupported" };

    if (Notification.permission !== "granted")
        return { ok: false, reason: "not_granted" };

    new Notification(title, { body, ...options });
    return { ok: true };
}
