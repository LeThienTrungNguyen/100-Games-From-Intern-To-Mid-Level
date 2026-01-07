using UnityEngine;

public static class WordData
{
    // Hàm lấy từ ngẫu nhiên
    public static string GetRandomWord()
    {
        return allWords[Random.Range(0, allWords.Length)];
    }

    // Danh sách ~600 từ tiếng Anh thông dụng (1-10 ký tự) được chọn lọc kỹ
    public static readonly string[] allWords = new string[]
    {
        // 2-3 Ký tự (Dễ)
        "ad", "go", "if", "at", "on", "he", "we", "in", "is", "it", 
        "to", "up", "us", "am", "be", "do", "no", "of", "my", "by", 
        "cat", "dog", "sun", "run", "sky", "cup", "bat", "hat", "red", "box",
        "eye", "ice", "jam", "key", "law", "mix", "net", "oil", "pie", "raw",
        "sad", "tea", "use", "van", "war", "zoo", "ant", "bus", "car", "day",
        "egg", "fan", "gas", "hen", "ink", "jet", "kit", "log", "man", "nut",
        "owl", "pen", "rat", "sea", "toy", "urn", "vet", "wet", "yak", "zip",
        "fox", "gem", "hip", "jaw", "kid", "lip", "mud", "nod", "oak", "paw",
        "rug", "saw", "tag", "urn", "vow", "wax", "yes", "zap", "art", "bed",
        "cap", "den", "ear", "fit", "gym", "hit", "job", "lab", "map", "nap",

        // 4 Ký tự (Trung bình - Thấp)
        "code", "data", "game", "hack", "java", "node", "ruby", "rust", "html", "link",
        "wifi", "byte", "file", "icon", "jpeg", "ping", "zoom", "clip", "drag", "font",
        "area", "army", "baby", "back", "ball", "band", "bank", "base", "bill", "body",
        "book", "call", "card", "care", "case", "cash", "city", "club", "cost", "date",
        "deal", "door", "duty", "east", "edge", "face", "fact", "farm", "fear", "fire",
        "fish", "food", "foot", "form", "fund", "gain", "girl", "goal", "gold", "hair",
        "half", "hall", "hand", "head", "help", "high", "hold", "home", "hope", "hour",
        "idea", "iron", "item", "join", "jump", "jury", "keep", "kick", "kill", "kind",
        "king", "lady", "land", "lane", "last", "lead", "left", "less", "life", "line",
        "list", "long", "look", "loop", "lord", "loss", "love", "luck", "mail", "main",
        
        // 5 Ký tự (Trung bình)
        "apple", "bread", "chair", "dance", "eagle", "fruit", "grape", "house", "image", "juice",
        "knife", "lemon", "mouse", "night", "ocean", "piano", "queen", "river", "snake", "table",
        "unity", "virus", "water", "xenon", "yacht", "zebra", "cloud", "drink", "earth", "flame",
        "ghost", "heart", "igloo", "joker", "koala", "light", "money", "nurse", "onion", "paper",
        "robot", "sugar", "tiger", "uncle", "video", "whale", "xerox", "youth", "zesty", "brave",
        "abuse", "adult", "agent", "anger", "award", "basis", "beach", "birth", "block", "blood",
        "board", "brain", "break", "brown", "buyer", "cause", "chain", "chest", "chief", "child",
        "china", "claim", "class", "clock", "coach", "coast", "court", "cover", "cream", "crime",
        "cross", "crowd", "crown", "cycle", "death", "depth", "doubt", "draft", "drama", "dream",
        "dress", "drive", "enemy", "entry", "error", "event", "faith", "fault", "field", "fight",

        // 6 Ký tự (Trung bình - Khá)
        "script", "server", "python", "docker", "binary", "client", "config", "design", "engine", "filter",
        "global", "header", "import", "kernel", "laptop", "matrix", "network", "object", "plugin", "query",
        "action", "agency", "agenda", "animal", "answer", "appeal", "artist", "attack", "author", "banana",
        "barrel", "basket", "battle", "beauty", "belief", "border", "bottle", "bottom", "branch", "breath",
        "bridge", "budget", "burden", "bureau", "button", "camera", "cancer", "canvas", "carbon", "career",
        "castle", "centre", "chance", "change", "charge", "choice", "church", "circle", "coffee", "column",
        "cookie", "copper", "corner", "cotton", "county", "couple", "course", "cousin", "credit", "crisis",
        "custom", "damage", "danger", "dealer", "debate", "decade", "defeat", "defic", "degree", "demand",
        "deputy", "desert", "device", "detail", "dinner", "doctor", "dollar", "domain", "double", "driver",

        // 7 Ký tự (Khó)
        "account", "address", "advance", "adviser", "airline", "airport", "alcohol", "analyst", "anxiety", "arrival",
        "article", "assault", "auction", "average", "backing", "balance", "balloon", "banking", "barrier", "battery",
        "bedroom", "benefit", "billion", "biscuit", "blanket", "cabinet", "caliber", "calling", "calorie", "captain",
        "capture", "carrier", "caution", "ceiling", "central", "century", "chamber", "channel", "chapter", "charger",
        "charity", "charlie", "charter", "chicken", "chimney", "circuit", "citizen", "classic", "climate", "closing",
        "closure", "clothes", "coating", "cockpit", "college", "comfort", "command", "comment", "compact", "company",
        "compass", "complex", "concept", "concern", "concert", "conduct", "conflic", "connect", "console", "contact",
        "content", "contest", "context", "control", "convert", "cooking", "cooling", "copying", "council", "counter",

        // 8-10 Ký tự (Rất khó / Boss)
        "absolute", "building", "calendar", "database", "elephant", "festival", "guardian", "hospital", "infinite", "junction",
        "keyboard", "language", "mountain", "notebook", "operator", "password", "question", "resource", "solution", "triangle",
        "umbrella", "vacation", "wildlife", "xylophone", "yourself", "zeppelin", "algorithm", "backspace", "compiler", "debugger",
        "emulator", "firewall", "gigabyte", "hardware", "internet", "joystick", "kilobyte", "listener", "megabyte", "navigate",
        "overflow", "platform", "quantity", "renderer", "security", "terminal", "username", "variable", "wireless", "zipper",
        "adventure", "apartment", "architect", "assistant", "associate", "attention", "attribute", "authority", "available", "beautiful",
        "blueprint", "breakfast", "brilliant", "broadcast", "candidate", "celebrate", "challenge", "character", "chocolate", "classroom",
        "colleague", "commander", "committee", "community", "complaint", "component", "condition", "confident", "confusion", "consensus",
        "construct", "container", "corporate", "counselor", "criticism", "curiosity", "dashboard", "dedication", "defendant", "democracy"
    };
}