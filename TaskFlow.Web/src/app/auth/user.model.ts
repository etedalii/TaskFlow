export class User {
  constructor(
    public name: string,
    public lastName: string,
    public role : string,
    public email: string,
    public id: string,
    private _token: string,
    private _tokenExpireAt: Date,
    private _refreshToken: string
  ) {}

  get token() {
    if (!this._tokenExpireAt || new Date() > this._tokenExpireAt) 
      {
        console.log('if check date is wrong')
        return null;}

    return this._token;
  }

  get refreshToken(){
    return this._refreshToken;
  }
}
