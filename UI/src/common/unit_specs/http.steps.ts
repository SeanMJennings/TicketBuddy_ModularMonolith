import {beforeEach, expect} from "vitest";
import {get, post, put, deleteCall} from '../http';
import {MockServer} from '../../testing/mock-server';
import {waitUntil} from '../../testing/utilities';

const mockServer = MockServer.New();
let wait_for_get: () => boolean;
let wait_for_post: () => boolean;
let wait_for_put: () => boolean;
let wait_for_delete: () => boolean;

beforeEach(() => {
    mockServer.reset();
    wait_for_get = mockServer.get(`/wibble`, { id: 'wobble' });
    wait_for_post = mockServer.post(`/wibble`, { id: 'wobble'});
    wait_for_put = mockServer.put(`/wibble`, { id: 'wobble' });
    wait_for_delete = mockServer.delete(`/wibble`);
    mockServer.start();
});

export async function make_get_request() {
    const idObject = await get<{ id: string }>("/wibble")
    await waitUntil(wait_for_get);
    expect(idObject.id).toBe('wobble');
}

export async function make_post_request() {
    const postBody = { test: 'wibble'};
    await post("/wibble", { test: 'wibble'})
    await waitUntil(wait_for_post);
    expect(mockServer.content).toStrictEqual(postBody);
}

export async function make_put_request() {
    const putBody = { test: 'wobble'};
    await put("/wibble", putBody);
    await waitUntil(wait_for_put);
    expect(mockServer.content).toStrictEqual(putBody);
}

export async function make_delete_request() {
    await deleteCall("/wibble");
    await waitUntil(wait_for_delete);
}