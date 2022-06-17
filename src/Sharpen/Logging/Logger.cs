
namespace Sharpen.Logging {

    public enum Level {
        Debug,
        Info,
        Warn,
        Error
    }

    public class Logger {

        public void Log(Level level, string msg, params object[] args){

        }

        public static Logger GetLogger(string name) {
            return null;
        }
    }
}
