export class NormalRegex {
    static readonly USERNAME_REGEX = "^[a-z0-9_-]{5,15}$";
    static readonly PASSWORD_REGEX =
      "^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{6,16}$";
    static readonly WEBSITE_REGEX =
      "https?://(www.)?[-a-zA-Z0-9@:%._+~#=]{2,256}.[a-z]{2,6}\b([-a-zA-Z0-9@:%_+.~#?&//=]*)";
  }