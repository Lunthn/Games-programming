// array that contains the paths to the house models
const houses = [
  "House1.glb",
  "House2.glb",
  "Two story house1.glb",
  "Two story house2.glb",
  "Two story house3.glb",
  "Two story house4.glb",
  "Two story house5.glb",
  "Two story house6.glb",
];

// initialize loaders
const gltfLoader = new THREE.GLTFLoader();
const textureLoader = new THREE.TextureLoader();

export async function buildHouse() {
  // randomly select a house from the array
  const randomHouseIndex = Math.floor(Math.random() * houses.length);
  const randomHouseString = houses[randomHouseIndex];

  // load the house
  const gltf = await gltfLoader.loadAsync("Models/Suburban Houses Pack-glb/" + randomHouseString)
  const house = gltf.scene;

  house.traverse((child) => {
    if (child.isMesh) {
      child.castShadow = true;
    }
  });

  return house;
}

export function buildFireHydrant() {
  const hydrantGroup = new THREE.Group();

  // create materials for the fire hydrant
  const redMaterial = new THREE.MeshStandardMaterial({
    color: 0xaa0000,
    roughness: 0.6,
  });
  const metalMaterial = new THREE.MeshStandardMaterial({
    color: 0x333333,
    metalness: 0.8,
    roughness: 0.2,
  });

  // create the fire hydrant body
  const bodyGeometry = new THREE.CylinderGeometry(0.5, 0.5, 1.2, 32);
  const body = new THREE.Mesh(bodyGeometry, redMaterial);
  body.castShadow = true;
  body.receiveShadow = true;
  hydrantGroup.add(body);

  // create various geometries that will be combined into a fire hydrant
  const topGeometry = new THREE.SphereGeometry(
    0.5,
    32,
    32,
    0,
    Math.PI * 2,
    0,
    Math.PI / 2,
  );
  const top = new THREE.Mesh(topGeometry, redMaterial);
  top.position.y = 0.6;
  top.castShadow = true;
  hydrantGroup.add(top);

  const nutGeometry = new THREE.CylinderGeometry(0.15, 0.15, 0.2, 6);
  const nut = new THREE.Mesh(nutGeometry, redMaterial);
  nut.position.y = 1.1;
  nut.castShadow = true;
  hydrantGroup.add(nut);

  const baseGeometry = new THREE.CylinderGeometry(0.65, 0.65, 0.2, 32);
  const base = new THREE.Mesh(baseGeometry, redMaterial);
  base.position.y = -0.6;
  base.castShadow = true;
  base.receiveShadow = true;
  hydrantGroup.add(base);

  const waterGeometry = new THREE.CylinderGeometry(0.2, 0.2, 0.4, 32);
  const water = new THREE.Mesh(waterGeometry, metalMaterial);
  water.rotation.x = Math.PI / 2;
  water.position.set(0, 0.2, 0.4);
  water.castShadow = true;
  hydrantGroup.add(water);

  hydrantGroup.scale.set(0.08, 0.08, 0.08);

  return hydrantGroup;
}

export function buildStreet(length) {
  const streetGroup = new THREE.Group();
  const posY = -0.12;
  const posZ = 0.75;

  // creating the texture of the streeet
  const streetTexture = textureLoader.load("Textures/street-texture.jpg");
  const streetMat = new THREE.MeshStandardMaterial({ map: streetTexture });

  // setting the properties of the texture
  streetTexture.rotation = Math.PI / 2;
  streetTexture.wrapS = THREE.RepeatWrapping;
  streetTexture.wrapT = THREE.RepeatWrapping;
  streetTexture.repeat.set(length / 10, 1);

  // creating the street itself
  const streetGeom = new THREE.BoxGeometry(length, 1, 0.01);
  const street = new THREE.Mesh(streetGeom, streetMat);
  street.rotation.x = -Math.PI / 2;
  street.position.y = posY;
  street.receiveShadow = true;
  streetGroup.add(street);

  // creating the texture of the sidewalk
  const sidewalkTexture = textureLoader.load("Textures/sidewalk-texture.jpg")
  const sidewalkMat = new THREE.MeshStandardMaterial({ map: sidewalkTexture })

  // setting the properties of the texture
  sidewalkTexture.wrapS = THREE.RepeatWrapping;
  sidewalkTexture.wrapT = THREE.RepeatWrapping;
  sidewalkTexture.repeat.set(length / 1, 1);

  // creating the sidewalk
  const sidewalkGeom = new THREE.BoxGeometry(length, .5, 0.03);
  const sidewalk = new THREE.Mesh(sidewalkGeom, sidewalkMat);
  sidewalk.rotation.x = -Math.PI / 2;
  sidewalk.receiveShadow = true;

  // duplicating sidewalk and setting positions
  const sidewalk2 = sidewalk.clone();
  sidewalk.position.y = posY;
  sidewalk2.position.y = posY;
  sidewalk.position.z = -posZ;
  sidewalk2.position.z = posZ;

  streetGroup.add(sidewalk);
  streetGroup.add(sidewalk2);

  return streetGroup;
}

export async function buildTree() {
  // load the tree model
  const gltf = await gltfLoader.loadAsync("Models/City Pack-glb/Tree.glb")
  const tree = gltf.scene;

  tree.traverse((child) => {
    if (child.isMesh) {
      child.castShadow = true;
    }
  });

  // adjust scaling to compensate for model size
  tree.scale.set(0.002, 0.002, 0.002);
  
  return tree;
}

export function buildSun(x, y, z) {
  // create sun geometry
  const sunGeometry = new THREE.SphereGeometry(15, 32, 16);
  const sunMaterial = new THREE.MeshBasicMaterial({ color: "yellow" });
  const sun = new THREE.Mesh(sunGeometry, sunMaterial);

  sun.scale.set(0.1, 0.1, 0.1);

  // create a light source
  const light = new THREE.DirectionalLight(0xffffff, 1);
  light.castShadow = true;
  light.shadow.mapSize.width = 1024;
  light.shadow.mapSize.height = 1024;
  light.intensity = 0.7;

  light.position.set(x, y, z);
  sun.position.set(x, y, z);

  return { sun, light };
}

export function buildMonument() {
  const monumentGroup = new THREE.Group();

  // create monument base geometry
  const baseGeometry = new THREE.CylinderGeometry(1.15, 1, 5, 32);
  const base = new THREE.Mesh(
    baseGeometry,
    new THREE.MeshPhongMaterial({ color: "grey" }),
  );
  base.castShadow = true;
  base.receiveShadow = true;
  monumentGroup.add(base);

  // create monument art geometry
  const artGeometry = new THREE.TorusGeometry(10, 3, 100, 16);
  const art = new THREE.Mesh(artGeometry, new THREE.MeshNormalMaterial());
  art.receiveShadow = true;
  art.castShadow = true;
  art.scale.setScalar(0.08);
  art.position.y = 4;
  monumentGroup.add(art);

  // this is what you think it is
  gltfLoader.load("Models/teapot.glb", (gltf) => {
    const teapot = gltf.scene;

    teapot.scale.setScalar(0.3);

    teapot.position.y = 3.8;

    teapot.traverse((child) => {
      if (child.isMesh) {
        child.castShadow = true;
        child.receiveShadow = true;
        child.material = new THREE.MeshNormalMaterial();
      }
    });
    monumentGroup.add(teapot);
  });

  monumentGroup.userData.update = (time) => {
    art.rotation.x += 0.01;
    art.rotation.y -= 0.01;
    art.rotation.z -= 0.01;
  };

  monumentGroup.scale.setScalar(0.3);

  return monumentGroup;
}

export function buildGround(boundary) {
  // create top layer (grass)
  const grass = new THREE.Mesh(
    new THREE.BoxGeometry(boundary * 2 + 1, 0.25, boundary * 2 + 1),
    new THREE.MeshStandardMaterial({ color: 0x6b8e23 }),
  );

  // create bottom layer (ground)
  const ground = new THREE.Mesh(
    new THREE.BoxGeometry(boundary * 2 + 1, 0.5, boundary * 2 + 1),
    new THREE.MeshStandardMaterial({ color: 0x654321 }),
  );

  // set properties
  grass.position.y = -0.25;
  ground.position.y = -0.6125;
  grass.receiveShadow = true;

  return { grass, ground };
}

export async function buildBench(){
  const gltf = await gltfLoader.loadAsync("Models/City Pack-glb/Bench.glb");
  const bench = gltf.scene;

  bench.traverse((child) => {
    if (child.isMesh) {
      child.castShadow = true;

      if(child.name == "Node-Mesh"){ // set material of planks
        child.material = new THREE.MeshLambertMaterial({ color: "brown" })
      }
      else if(child.name == "Node-Mesh_1"){ // set material of stand
        child.material = new THREE.MeshStandardMaterial({ color: "white", metalness: 1, roughness: 1 })
      }
    }
  });

  bench.scale.setScalar(0.15);
  bench.position.set(0, 0.025, 0.925);

  return bench;
}

export async function buildTrashCan(){
  const gltf = await gltfLoader.loadAsync("Models/City Pack-glb/Trash Can.glb");
  const trashCan = gltf.scene;

  trashCan.traverse((child) => {
    if(child.isMesh) {
      child.castShadow = true;
      child.material = new THREE.MeshStandardMaterial({ color: "gray", metalness: 0, roughness: 1 });
    }
  });

  trashCan.scale.setScalar(0.05);
  trashCan.position.set(0.35, -0.01, 0.925);

  return trashCan;
}