export async function startTimer(duration, callback) {
    const startTime = Date.now();

    const timer = setInterval(() => {
        const elapsedTime = Date.now() - startTime;
        if (elapsedTime >= duration) {
            clearInterval(timer);
            if (typeof callback === 'function') {
                callback();
            }
        }
    }, 1000); 
}