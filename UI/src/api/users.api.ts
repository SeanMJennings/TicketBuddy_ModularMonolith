﻿import type {User} from "../domain/user.ts";
import {get} from "../common/http.ts";

export const getUsers = () => {
    return get<User[]>('/users');
}